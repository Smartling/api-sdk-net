namespace Smartling.Api
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Text;

  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;

  using Smartling.Api.Model;

  public class Translator : ITranslator
  {
    public const string LocalesSeparator = "|";

    private const string UploadUrl = "/file/upload";
    private const string GetUrl = "/file/get";
    private const string ListUrl = "/file/list";
    private const string StatusUrl = "/file/status";
    private const string DeleteUrl = "/file/delete";

    private const string ApiKeyParameterName = "apiKey";
    private const string ProjectIdParameterName = "projectId";
    private const string FileUriParameterName = "fileUri";
    private const string FileTypeParameterName = "fileType";
    private const string FileNameParameterName = "file";
    private const string LocaleParameterName = "locale";
    private const string RetrievalTypeParameterName = "retrievalType";
    private const string CallbackUrlParameterName = "callbackUrl";
    private const string LocalesToApproveParameterName = "localesToApprove";
    private const string OverwriteApprovedLocalesParameterName = "overwriteApprovedLocales";

    private const string GetMethodName = "GET";
    private const string DeleteMethodName = "DELETE";
    private const string PostMethodName = "POST";

    private readonly string apiUrl;
    private readonly string apiKey;
    private readonly string projectId;
    private readonly string callbackUrl;
    
    static Translator()
    {
      // Smartling works only with TLS and doesn't accepts SSL3
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
    }

    public Translator(string apiUrl, string apiKey, string projectId, string callbackUrl)
    {
      this.apiUrl = apiUrl;
      this.apiKey = apiKey;
      this.projectId = projectId;
      this.callbackUrl = callbackUrl;
    }

    public FileStatus UploadFile(string filePath, string fileUri, string fileType, string approvedLocales, bool authorizeContent)
    {
      var uriBuilder = this.GetRequestStringBuilder(UploadUrl)
        .AppendFormat("&{0}={1}", FileUriParameterName, fileUri)
        .AppendFormat("&{0}={1}", FileTypeParameterName, fileType);

      if (!string.IsNullOrEmpty(this.callbackUrl))
      {
        uriBuilder.AppendFormat("&{0}={1}", CallbackUrlParameterName, this.callbackUrl);
      }

      if (authorizeContent)
      {
        uriBuilder.AppendFormat("&{0}={1}", OverwriteApprovedLocalesParameterName, false);
        var locales = approvedLocales.Split(new[] { LocalesSeparator.First() }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < locales.Length; i++)
        {
          uriBuilder.AppendFormat("&{0}[{1}]={2}", LocalesToApproveParameterName, i, locales[i]);
        }
      }

      var jsonResponse = GetResponse(PreparePostRequest(uriBuilder.ToString(), filePath));
      JObject response = JObject.Parse(jsonResponse);
      var fileStatus = JsonConvert.DeserializeObject<FileStatus>(response["response"]["data"].ToString());

      return fileStatus;
    }

    public virtual string GetFile(string fileUri, string locale, string retrievalType)
    {
      var uriBuilder = this.GetRequestStringBuilder(GetUrl)
        .AppendFormat("&{0}={1}", FileUriParameterName, fileUri)
        .AppendFormat("&{0}={1}", LocaleParameterName, locale)
        .AppendFormat("&{0}={1}", RetrievalTypeParameterName, retrievalType);

      return GetResponse(PrepareGetRequest(uriBuilder.ToString()));
    }

    public Stream GetFileStream(string fileUri, string locale)
    {
      var uriBuilder = this.GetRequestStringBuilder(GetUrl)
        .AppendFormat("&{0}={1}", FileUriParameterName, fileUri)
        .AppendFormat("&{0}={1}", LocaleParameterName, locale);

      return GetResponseStream(PrepareGetRequest(uriBuilder.ToString()));
    }

    public IEnumerable<FileStatus> GetFilesList(string locale)
    {
      var uriBuilder = this.GetRequestStringBuilder(ListUrl);
      uriBuilder.AppendFormat("&{0}={1}", LocaleParameterName, locale);

      var jsonResponse = GetResponse(PrepareGetRequest(uriBuilder.ToString()));
      JObject response = JObject.Parse(jsonResponse);
      var fileList = JsonConvert.DeserializeObject<FileList>(response["response"]["data"].ToString());

      return fileList.fileList;
    }

    public FileStatus GetFileStatus(string fileUri, string locale)
    {
      var uriBuilder = this.GetRequestStringBuilder(StatusUrl)
        .AppendFormat("&{0}={1}", FileUriParameterName, fileUri)
        .AppendFormat("&{0}={1}", LocaleParameterName, locale);

      var jsonResponse = GetResponse(PrepareGetRequest(uriBuilder.ToString()));

      JObject response = JObject.Parse(jsonResponse);
      var fileStatus = JsonConvert.DeserializeObject<FileStatus>(response["response"]["data"].ToString());

      return fileStatus;
    }

    public void DeleteFile(string fileUri)
    {
      var uriBuilder = this.GetRequestStringBuilder(DeleteUrl)
        .AppendFormat("&{0}={1}", FileUriParameterName, fileUri);

      GetResponse(PrepareDeleteRequest(uriBuilder.ToString()));
    }

    public virtual WebRequest PreparePostRequest(string uri, string filePath)
    {
      var boundary = string.Format("----------{0:N}", Guid.NewGuid());

      var request = WebRequest.Create(uri);
      request.Method = PostMethodName;
      request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

      using (var memoryStream = new MemoryStream())
      {
        var header = new StringBuilder();
        header.AppendFormat("--{0}\r\n", boundary);
        header.AppendFormat("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n", FileNameParameterName, filePath);
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

    public virtual WebRequest PrepareGetRequest(string uri)
    {
      var request = WebRequest.Create(uri);
      request.Method = GetMethodName;

      return request;
    }

    private static WebRequest PrepareDeleteRequest(string uri)
    {
      var request = WebRequest.Create(uri);
      request.Method = DeleteMethodName;

      return request;
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
            string messages = error.response.messages.Aggregate(
              string.Empty,
              (current, message) => current + Environment.NewLine + message);

            throw new Exception(error.response.code + ": " + messages, e);
          }
        }
      }
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

    private StringBuilder GetRequestStringBuilder(string methodUrl)
    {
      return
        new StringBuilder().AppendFormat(this.apiUrl + methodUrl)
          .AppendFormat("?{0}={1}", ApiKeyParameterName, this.apiKey)
          .AppendFormat("&{0}={1}", ProjectIdParameterName, this.projectId);
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
  }
}