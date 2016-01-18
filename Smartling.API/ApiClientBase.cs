using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Smartling.Api.Exceptions;
using Smartling.Api.Model;

namespace Smartling.Api
{
  public class ApiClientBase
  {
    protected const string DEFAULT_API_GATEWAY_URL = "https://api.smartling.com";
    private const string JsonContentType = "application/json; charset=UTF-8";
    private const string JsonAccept = "application/json";
    private const string FileNameParameterName = "file";
    public string ApiGatewayUrl { get; set; }

    private ClientUid apiClientUid;
    public ClientUid ApiClientUid
    {
      get
      {
        return apiClientUid ?? ClientUid.DefaultUid();
      }

      set { apiClientUid = value; }
    }

    public ApiClientBase()
    {
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
    }

    public virtual string GetResponse(WebRequest request)
    {
      try
      {
        using (var response = request.GetResponse())
        using (var stream = response.GetResponseStream())
        using (var reader = new StreamReader(stream))
        {
          return reader.ReadToEnd();
        }
      }
      catch (WebException e)
      {
        if (e.Response == null)
        {
          throw;
        }

        using (WebResponse response = e.Response)
        {
          using (Stream data = response.GetResponseStream())
          using (var reader = new StreamReader(data))
          {
            string text = reader.ReadToEnd();
            var error = JsonConvert.DeserializeObject<Error>(text);
            string messages = error.response.errors.Aggregate(
              string.Empty,
              (current, message) => string.Format("{0}{1}{2} ({3})", current, Environment.NewLine, message.message, message.details));

            if (messages.Contains("HTTP 401 Unauthorized"))
            {
              throw new AuthorizationException(error.response.code + ": " + messages, e);
            }

            throw new Exception(error.response.code + ": " + messages, e);
          }
        }
      }
    }

    public virtual WebRequest PrepareJsonPostRequest(string url, object command)
    {
      var request = (HttpWebRequest)WebRequest.Create(url);
      var json = JsonConvert.SerializeObject(command);
      byte[] postBytes = Encoding.UTF8.GetBytes(json);

      request.Method = WebRequestMethods.Http.Post;
      request.ContentType = JsonContentType;
      request.Accept = JsonAccept;
      request.ContentLength = postBytes.Length;

      Stream requestStream = request.GetRequestStream();
      requestStream.Write(postBytes, 0, postBytes.Length);
      requestStream.Close();
      return request;
    }

    public virtual WebRequest PrepareFilePostRequest(string uri, string filePath, NameValueCollection formData, string token)
    {
      var boundary = string.Format("----------{0:N}", Guid.NewGuid());

      var request = WebRequest.Create(uri);
      request.Method = WebRequestMethods.Http.Post;
      request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

      using (var memoryStream = new MemoryStream())
      {
        // Adding form data
        string formDataHeaderTemplate = Environment.NewLine + "--" + boundary + Environment.NewLine +
                                        "Content-Disposition: form-data; name=\"{0}\";" + Environment.NewLine +
                                        Environment.NewLine + "{1}";

        foreach (string key in formData.Keys)
        {
          byte[] formItemBytes = Encoding.UTF8.GetBytes(string.Format(formDataHeaderTemplate, key, formData[key]));
          memoryStream.Write(formItemBytes, 0, formItemBytes.Length);
        }

        // Adding file contents
        var header = new StringBuilder();
        header.AppendFormat("\r\n--{0}\r\n", boundary);
        header.AppendFormat("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n", FileNameParameterName,
          filePath);
        header.Append("Content-Type: application/octet-stream\r\n\r\n");

        var headerData = Encoding.UTF8.GetBytes(header.ToString());
        memoryStream.Write(headerData, 0, headerData.Length);

        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
          var buffer = new byte[1024];
          int bytesRead;
          while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
          {
            memoryStream.Write(buffer, 0, bytesRead);
          }
        }

        var footerData = Encoding.UTF8.GetBytes(string.Format("\r\n--{0}--\r\n", boundary));
        memoryStream.Write(footerData, 0, footerData.Length);

        request.ContentLength = memoryStream.Length;
        request.Headers.Add("Authorization", "Bearer " + token);

        using (var stream = request.GetRequestStream())
        {
          memoryStream.Position = 0;

          var buffer = new byte[memoryStream.Length];
          memoryStream.Read(buffer, 0, buffer.Length);

          stream.Write(buffer, 0, buffer.Length);
        }
      }

      return request;
    }

    public virtual WebRequest PrepareGetRequest(string uri, string token)
    {
      var request = WebRequest.Create(uri);
      request.Method = WebRequestMethods.Http.Get;
      request.Headers.Add("Authorization", "Bearer " + token);

      return request;
    }

    public virtual WebRequest PreparePutRequest(string uri, string token)
    {
      var request = WebRequest.Create(uri);
      request.Method = WebRequestMethods.Http.Put;
      request.Headers.Add("Authorization", "Bearer " + token);
      var boundary = string.Format("----------{0:N}", Guid.NewGuid());
      request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
      return request;
    }

    public virtual WebRequest PrepareDeleteRequest(string uri, string token)
    {
      var request = WebRequest.Create(uri);
      request.Method = WebRequestMethods.Http.Post;
      request.Headers.Add("Authorization", "Bearer " + token);
      var boundary = string.Format("----------{0:N}", Guid.NewGuid());
      request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
      return request;
    }

    public Stream GetResponseStream(WebRequest request)
    {
      try
      {
        using (var response = request.GetResponse())
        using (var stream = response.GetResponseStream())
        {
          var data = ReadFully(stream);
          return new MemoryStream(data);
        }
      }
      catch (WebException e)
      {
        if (e.Response == null)
        {
          throw;
        }

        using (WebResponse response = e.Response)
        {
          using (Stream data = response.GetResponseStream())
          using (var reader = new StreamReader(data))
          {
            string text = reader.ReadToEnd();
            var error = JsonConvert.DeserializeObject<Error>(text);
            string messages = error.response.messages.Aggregate(
              string.Empty,
              (current, message) => current + Environment.NewLine + message);

            throw new Exception(error.response.code + ": " + messages, e);
          }
        }
      }
    }

    public static byte[] ReadFully(Stream stream)
    {
      var buffer = new byte[32768];
      using (var ms = new MemoryStream())
      {
        while (true)
        {
          int read = stream.Read(buffer, 0, buffer.Length);
          if (read <= 0)
          {
            return ms.ToArray();
          }

          ms.Write(buffer, 0, read);
        }
      }
    }

    protected StringBuilder GetRequestStringBuilder(string methodUrl)
    {
      if (string.IsNullOrEmpty(ApiGatewayUrl))
      {
        return new StringBuilder().AppendFormat(DEFAULT_API_GATEWAY_URL + methodUrl);
      }

      return new StringBuilder().AppendFormat(ApiGatewayUrl + methodUrl);
    }
  }
}