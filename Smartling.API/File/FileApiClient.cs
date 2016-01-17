using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smartling.Api.Authentication;
using Smartling.Api.Exceptions;
using Smartling.Api.Model;

namespace Smartling.Api.File
{
  public class FileApiClient : ApiClientBase
  {
    private readonly string UploadUrl = "/files-api/v2/projects/{0}/file";
    private const string GetUrl = "/files-api/v2/projects/{0}/locales/{1}/file";
    private const string ListUrl = "/files-api/v2/projects/{0}/files/list";
    private const string StatusUrl = "/files-api/v2/projects/{0}/file/status";
    private const string StatusDetailUrl = "/files-api/v2/projects/{0}/locales/{1}/file/status";
    private const string LastModifiedUrl = "/files-api/v2/projects/{0}/file/last-modified";
    private const string LastModifiedDetailUrl = "/files-api/v2/projects/{0}/locales/{1}/file/last-modified";
    private const string ContentAuthorizationUrl = "/files-api/v2/projects/{0}/file/authorized-locales";
    private const string DeleteUrl = "/files-api/v2/projects/{0}/file/delete";
    
    private const string FileUriParameterName = "fileUri";
    private const string LocaleIdsParameterName = "localeIds[]";
    private const string FileTypeParameterName = "fileType";
    private const string RetrievalTypeParameterName = "retrievalType";
    private const string CallbackUrlParameterName = "callbackUrl";
    private const string LocalesToApproveParameterName = "localeIdsToAuthorize[]";

    private readonly string projectId;
    private readonly string callbackUrl;
    private readonly IAuthenticationStrategy auth;

    public FileApiClient(IAuthenticationStrategy auth, string projectId, string callbackUrl)
    {
      this.auth = auth;
      this.UploadUrl = string.Format(this.UploadUrl, projectId);
      this.projectId = projectId;
      this.callbackUrl = callbackUrl;
    }

    /// <summary>
    /// Uploads specified file to Smartling
    /// </summary>
    /// <param name="filePath">Path to the file</param>
    /// <param name="fileUri">Uri of the file as it will appear in </param>
    /// <param name="fileType">File type, used by Smartling</param>
    /// <param name="approvedLocales">A list of locales separated by ','. Example: "ru-RU,fr-FR"</param>
    /// <param name="authorizeContent">By default, content needs to be approved before translating</param>
    /// <returns></returns>
    public FileUploadResult UploadFile(string filePath, string fileUri, string fileType, string approvedLocales, bool authorizeContent)
    {
      var uriBuilder = this.GetRequestStringBuilder(UploadUrl);
      var formData = new NameValueCollection();
      formData.Add(FileUriParameterName, fileUri);
      formData.Add(FileTypeParameterName, fileType);

      if (!string.IsNullOrEmpty(this.callbackUrl))
      {
        formData.Add(CallbackUrlParameterName, this.callbackUrl);
      }

      if (authorizeContent)
      {
        formData.Add(LocalesToApproveParameterName, approvedLocales);
      }

      return ExecuteUploadRequest(filePath, uriBuilder, formData);
    }

    private FileUploadResult ExecuteUploadRequest(string filePath, StringBuilder uriBuilder, NameValueCollection formData)
    {
      try
      {
        var request = PrepareFilePostRequest(uriBuilder.ToString(), filePath, formData, auth.GetToken());
        var response = JObject.Parse(GetResponse(request));
        return JsonConvert.DeserializeObject<FileUploadResult>(response["response"]["data"].ToString());
      }
      catch (AuthorizationException)
      {
        var request = PrepareFilePostRequest(uriBuilder.ToString(), filePath, formData, auth.GetToken(true));
        var response = JObject.Parse(GetResponse(request));
        return JsonConvert.DeserializeObject<FileUploadResult>(response["response"]["data"].ToString());
      }
    }

    public string GetFile(string fileUri, string locale, string retrievalType)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(GetUrl, projectId, "ru-RU"))
        .AppendFormat("?{0}={1}", FileUriParameterName, fileUri)
        .AppendFormat("&{0}={1}", RetrievalTypeParameterName, retrievalType);
      
      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      try
      {
        return GetResponse(request);
      }
      catch (AuthorizationException)
      {
        request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken(true));
        return GetResponse(request);
      }
    }

    // Retry the request in case of authentication error
    private JObject ExecuteRequest(WebRequest request, StringBuilder uriBuilder)
    {
      JObject response;
      try
      {
        response = JObject.Parse(GetResponse(request));
      }
      catch (AuthorizationException)
      {
        request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken(true));
        response = JObject.Parse(GetResponse(request));
      }

      return response;
    }

    public IEnumerable<FileStatus> GetFilesList()
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(ListUrl, projectId));

      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteRequest(request, uriBuilder);
      var fileList = JsonConvert.DeserializeObject<FileList>(response["response"]["data"].ToString());
      return fileList.items;
    }
    
    public FileStatus GetFileStatus(string fileUri)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(StatusUrl, projectId))
        .AppendFormat("?{0}={1}", FileUriParameterName, fileUri);

      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteRequest(request, uriBuilder);
      return JsonConvert.DeserializeObject<FileStatus>(response["response"]["data"].ToString());
    }

    public FileStatusDetail GetFileStatus(string fileUri, string locale)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(StatusDetailUrl, projectId, locale))
        .AppendFormat("?{0}={1}", FileUriParameterName, fileUri);
      
      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteRequest(request, uriBuilder);
      var fileStatus = JsonConvert.DeserializeObject<FileStatusDetail>(response["response"]["data"].ToString());

      return fileStatus;
    }
    
    public LastModified GetLastModified(string fileUri)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(LastModifiedUrl, projectId))
        .AppendFormat("?{0}={1}", FileUriParameterName, fileUri);

      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteRequest(request, uriBuilder);
      return JsonConvert.DeserializeObject<LastModified>(response["response"]["data"].ToString());
    }

    public LastModifiedDetail GetLastModified(string fileUri, string locale)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(LastModifiedDetailUrl, projectId, locale))
        .AppendFormat("?{0}={1}", FileUriParameterName, fileUri);

      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteRequest(request, uriBuilder);
      return JsonConvert.DeserializeObject<LastModifiedDetail>(response["response"]["data"].ToString());
    }

    public void Authorize(string fileUri, string approvedLocales)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(ContentAuthorizationUrl, projectId))
        .AppendFormat("?{0}={1}", FileUriParameterName, fileUri)
        .AppendFormat("&{0}={1}", LocaleIdsParameterName, approvedLocales);
      
      var request = PreparePutRequest(uriBuilder.ToString(), auth.GetToken());
      ExecuteRequest(request, uriBuilder);
    }

    public void Unauthorize(string fileUri, string removedLocales)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(ContentAuthorizationUrl, projectId))
        .AppendFormat("?{0}={1}", FileUriParameterName, fileUri)
        .AppendFormat("&{0}={1}", LocaleIdsParameterName, removedLocales);

      var request = PrepareDeleteRequest(uriBuilder.ToString(), auth.GetToken());
      ExecuteRequest(request, uriBuilder);
    }

    public void DeleteFile(string fileUri)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(DeleteUrl, projectId))
        .AppendFormat("?{0}={1}", FileUriParameterName, fileUri);

      var request = PrepareDeleteRequest(uriBuilder.ToString(), auth.GetToken());
      ExecuteRequest(request, uriBuilder);
    }
  }
}
