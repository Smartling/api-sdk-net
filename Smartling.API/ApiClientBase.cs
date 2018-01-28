using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smartling.Api.Authentication;
using Smartling.Api.Exceptions;
using Smartling.Api.Model;

namespace Smartling.Api
{
  public class ApiClientBase
  {
    private const string DEFAULT_API_GATEWAY_URL = "https://api.smartling.com";
    private const string JsonContentType = "application/json; charset=UTF-8";
    private const string JsonAccept = "application/json";
    private const string FileNameParameterName = "file";
    private const string AuthenticationErrorCode = "AUTHENTICATION_ERROR";
    private const string MaintenanceModeErrorCode = "MAINTENANCE_MODE_ERROR";
    private string apiGatewayUrl;

    public string ApiGatewayUrl
    {
      get
      {
        if (string.IsNullOrEmpty(apiGatewayUrl))
        {
          return DEFAULT_API_GATEWAY_URL;
        }

        return apiGatewayUrl;
      }
      set { apiGatewayUrl = value; }
    }

    private ClientUid apiClientUid;
    private string AuthorizationHeaderName = "Authorization";

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
      System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
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

            if (messages.Contains("HTTP 401 Unauthorized") || error.response.code == AuthenticationErrorCode)
            {
              throw new AuthenticationException(error.response.code + ": " + messages, e);
            }

            if (error.response.code == MaintenanceModeErrorCode)
            {
              throw new MaintenanceModeException(error.response.code + ": " + messages, e);
            }

            throw new Exception(error.response.code + ": " + messages, e);
          }
        }
      }
    }

    public virtual WebRequest PrepareJsonPostRequest(string url, object command, string token)
    {
      var request = (HttpWebRequest)WebRequest.Create(url);
      var json = JsonConvert.SerializeObject(command);
      if (command is string)
      {
        json = command as string;
      }

      byte[] postBytes = Encoding.UTF8.GetBytes(json);

      request.Method = WebRequestMethods.Http.Post;
      request.ContentType = JsonContentType;
      request.Accept = JsonAccept;
      request.ContentLength = postBytes.Length;
      request.UserAgent = ApiClientUid.ToUserAgent();

      if (!string.IsNullOrEmpty(token))
      {
        request.Headers.Add(AuthorizationHeaderName, "Bearer " + token);
      }

      Stream requestStream = request.GetRequestStream();
      requestStream.Write(postBytes, 0, postBytes.Length);
      requestStream.Close();
      return request;
    }

    public virtual WebRequest PrepareJsonPutRequest(string url, object command, string token)
    {
      var request = (HttpWebRequest)WebRequest.Create(url);
      var json = JsonConvert.SerializeObject(command);
      byte[] postBytes = Encoding.UTF8.GetBytes(json);

      request.Method = WebRequestMethods.Http.Put;
      request.ContentType = JsonContentType;
      request.Accept = JsonAccept;
      request.ContentLength = postBytes.Length;
      request.UserAgent = ApiClientUid.ToUserAgent();

      if (!string.IsNullOrEmpty(token))
      {
        request.Headers.Add(AuthorizationHeaderName, "Bearer " + token);
      }

      Stream requestStream = request.GetRequestStream();
      requestStream.Write(postBytes, 0, postBytes.Length);
      requestStream.Close();
      return request;
    }

    public virtual WebRequest PrepareFilePostRequest(string uri, string fileName, Stream fileStream, NameValueCollection formData, string token)
    {
      var boundary = string.Format("----------{0:N}", Guid.NewGuid());

      var request = (HttpWebRequest)WebRequest.Create(uri);
      request.Method = WebRequestMethods.Http.Post;
      request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
      request.UserAgent = ApiClientUid.ToUserAgent();

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
          fileName);
        header.Append("Content-Type: application/octet-stream\r\n\r\n");

        var headerData = Encoding.UTF8.GetBytes(header.ToString());
        memoryStream.Write(headerData, 0, headerData.Length);

        var fileBuffer = new byte[1024];
        int bytesRead;
        while ((bytesRead = fileStream.Read(fileBuffer, 0, fileBuffer.Length)) != 0)
        {
          memoryStream.Write(fileBuffer, 0, bytesRead);
        }

        var footerData = Encoding.UTF8.GetBytes(string.Format("\r\n--{0}--\r\n", boundary));
        memoryStream.Write(footerData, 0, footerData.Length);

        request.ContentLength = memoryStream.Length;
        request.Headers.Add(AuthorizationHeaderName, "Bearer " + token);

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
      var request = (HttpWebRequest)WebRequest.Create(uri);
      request.Method = WebRequestMethods.Http.Get;
      request.Headers.Add(AuthorizationHeaderName, "Bearer " + token);
      request.UserAgent = ApiClientUid.ToUserAgent();

      return request;
    }

    public virtual WebRequest PreparePutRequest(string uri, string token)
    {
      var request = (HttpWebRequest)WebRequest.Create(uri);
      request.Method = WebRequestMethods.Http.Put;
      request.Headers.Add(AuthorizationHeaderName, "Bearer " + token);
      request.UserAgent = ApiClientUid.ToUserAgent();

      var boundary = string.Format("----------{0:N}", Guid.NewGuid());
      request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
      return request;
    }

    public virtual WebRequest PrepareDeleteRequest(string uri, string token)
    {
      var request = (HttpWebRequest)WebRequest.Create(uri);
      request.Method = WebRequestMethods.Http.Post;
      request.Headers.Add(AuthorizationHeaderName, "Bearer " + token);
      request.UserAgent = ApiClientUid.ToUserAgent();
      var boundary = string.Format("----------{0:N}", Guid.NewGuid());
      request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
      return request;
    }
    
    // Retry the request in case of authentication error
    protected JObject ExecuteGetRequest(WebRequest request, StringBuilder uriBuilder, IAuthenticationStrategy auth)
    {
      JObject response;
      try
      {
        response = JObject.Parse(GetResponse(request));
      }
      catch (AuthenticationException)
      {
        request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken(true));
        response = JObject.Parse(GetResponse(request));
      }

      return response;
    }

    // Retry the request in case of authentication error
    protected JObject ExecutePostRequest(StringBuilder uriBuilder, object command, IAuthenticationStrategy auth)
    {
      JObject response;
      var request = PrepareJsonPostRequest(uriBuilder.ToString(), command, auth.GetToken());

      try
      {
        response = JObject.Parse(GetResponse(request));
      }
      catch (AuthenticationException)
      {
        request = PrepareJsonPostRequest(uriBuilder.ToString(), command, auth.GetToken(true));
        response = JObject.Parse(GetResponse(request));
      }

      return response;
    }

    // Retry the request in case of authentication error
    protected JObject ExecutePutRequest(StringBuilder uriBuilder, object command, IAuthenticationStrategy auth)
    {
      JObject response;
      var request = PrepareJsonPutRequest(uriBuilder.ToString(), command, auth.GetToken());

      try
      {
        response = JObject.Parse(GetResponse(request));
      }
      catch (AuthenticationException)
      {
        request = PrepareJsonPutRequest(uriBuilder.ToString(), command, auth.GetToken(true));
        response = JObject.Parse(GetResponse(request));
      }

      return response;
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
      return new StringBuilder().AppendFormat(ApiGatewayUrl + methodUrl);
    }
  }
}