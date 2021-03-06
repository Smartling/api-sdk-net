﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smartling.Api.Authentication;
using Smartling.Api.Exceptions;
using Smartling.Api.Extensions;
using Smartling.Api.Model;

namespace Smartling.Api.File
{
  public class FileApiClient : ApiClientBase
  {
    private const string UploadUrl = "/files-api/v2/projects/{0}/file";
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
    private const string NameSpaceParameterName = "smartling.namespace";
    private const string RetrievalTypeParameterName = "retrievalType";
    private const string CallbackUrlParameterName = "callbackUrl";
    private const string LocalesToApproveParameterName = "localeIdsToAuthorize[]";
    private const string CliendUidParameterName = "smartling.client_lib_id";

    private readonly string projectId;
    private readonly string callbackUrl;
    private readonly IAuthenticationStrategy auth;

    public FileApiClient(IAuthenticationStrategy auth, string projectId, string callbackUrl)
    {
      this.auth = auth;
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
    /// <param name="nameSpace">Assign a custom namespace to a file using the smartling.namespace API directive when you upload the file. 
    /// You may wish to do this if your File URI contains version information, to avoid creating a full set of unique strings every time you update a file.</param>
    /// <returns></returns>
    public virtual FileUploadResult UploadFile(string filePath, string fileUri, string fileType, string approvedLocales, bool authorizeContent, string nameSpace = null)
    {
      using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
      {
        return UploadFileStream(fileStream, fileUri, fileType, approvedLocales, authorizeContent, nameSpace);
      }
    }

    public virtual FileUploadResult UploadFileContents(string fileContents, string fileUri, string fileType, string approvedLocales, bool authorizeContent, string nameSpace = null)
    {
      using (var fileStream = fileContents.ToStream())
      {
        return UploadFileStream(fileStream, fileUri, fileType, approvedLocales, authorizeContent, nameSpace);
      }
    }

    public virtual FileUploadResult UploadFileStream(Stream fileStream, string fileUri, string fileType, string approvedLocales, bool authorizeContent, string nameSpace = null)
    {
      var uriBuilder = this.GetRequestStringBuilder(UploadUrl.FormatUri(projectId));
      var formData = new NameValueCollection();
      formData.Add(FileUriParameterName, fileUri);
      formData.Add(FileTypeParameterName, fileType);
      formData.Add(CliendUidParameterName, JsonConvert.SerializeObject(this.ApiClientUid));

      if (!string.IsNullOrEmpty(this.callbackUrl))
      {
        formData.Add(CallbackUrlParameterName, this.callbackUrl);
      }

      if (authorizeContent)
      {
        formData.Add(LocalesToApproveParameterName, approvedLocales);
      }

      if (!string.IsNullOrEmpty(nameSpace))
      {
        formData.Add(NameSpaceParameterName, nameSpace);
      }

      return ExecuteUploadRequest(fileStream, fileUri, uriBuilder, formData);
    }

    private FileUploadResult ExecuteUploadRequest(Stream fileStream, string fileUri, StringBuilder uriBuilder, NameValueCollection formData)
    {
      try
      {
        var request = PrepareFilePostRequest(uriBuilder.ToString(), fileUri, fileStream, formData, auth.GetToken());
        var response = JObject.Parse(GetResponse(request));
        return JsonConvert.DeserializeObject<FileUploadResult>(response["response"]["data"].ToString());
      }
      catch (AuthenticationException)
      {
        var request = PrepareFilePostRequest(uriBuilder.ToString(), fileUri, fileStream, formData, auth.GetToken(true));
        var response = JObject.Parse(GetResponse(request));
        return JsonConvert.DeserializeObject<FileUploadResult>(response["response"]["data"].ToString());
      }
    }

    public virtual string GetFile(string fileUri, string locale, string retrievalType)
    {
      var uriBuilder = this.GetRequestStringBuilder(GetUrl.FormatUri(projectId, locale))
        .AppendUri("?{0}={1}", FileUriParameterName, fileUri)
        .AppendFormat("&{0}={1}", RetrievalTypeParameterName, retrievalType);
      
      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      try
      {
        return GetResponse(request);
      }
      catch (AuthenticationException)
      {
        request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken(true));
        return GetResponse(request);
      }
    }

    public virtual IEnumerable<FileStatus> GetFilesList()
    {
      var uriBuilder = this.GetRequestStringBuilder(ListUrl.FormatUri(projectId));

      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteGetRequest(request, uriBuilder, auth);
      var fileList = JsonConvert.DeserializeObject<FileList>(response["response"]["data"].ToString());
      return fileList.items;
    }
    
    public virtual FileStatus GetFileStatus(string fileUri)
    {
      var uriBuilder = this.GetRequestStringBuilder(StatusUrl.FormatUri(projectId))
        .AppendUri("?{0}={1}", FileUriParameterName, fileUri);

      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteGetRequest(request, uriBuilder, auth);
      var result = JsonConvert.DeserializeObject<FileStatus>(response["response"]["data"].ToString());
      if (result != null && result.items != null)
      {
        foreach (var item in result.items)
        {
          item.totalStringCount = result.totalStringCount;
        }
      }

      return result;
    }

    public virtual FileStatusDetail GetFileStatus(string fileUri, string locale)
    {
      var uriBuilder = this.GetRequestStringBuilder(StatusDetailUrl.FormatUri(projectId, locale))
        .AppendUri("?{0}={1}", FileUriParameterName, fileUri);
      
      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteGetRequest(request, uriBuilder, auth);
      var fileStatus = JsonConvert.DeserializeObject<FileStatusDetail>(response["response"]["data"].ToString());

      return fileStatus;
    }
    
    public virtual LastModified GetLastModified(string fileUri)
    {
      var uriBuilder = this.GetRequestStringBuilder(LastModifiedUrl.FormatUri(projectId))
        .AppendUri("?{0}={1}", FileUriParameterName, fileUri);

      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteGetRequest(request, uriBuilder, auth);
      return JsonConvert.DeserializeObject<LastModified>(response["response"]["data"].ToString());
    }

    public virtual LastModifiedDetail GetLastModified(string fileUri, string locale)
    {
      var uriBuilder = this.GetRequestStringBuilder(LastModifiedDetailUrl.FormatUri(projectId, locale))
        .AppendUri("?{0}={1}", FileUriParameterName, fileUri);

      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteGetRequest(request, uriBuilder, auth);
      return JsonConvert.DeserializeObject<LastModifiedDetail>(response["response"]["data"].ToString());
    }

    public virtual void Authorize(string fileUri, string approvedLocales)
    {
      var uriBuilder = this.GetRequestStringBuilder(ContentAuthorizationUrl.FormatUri(projectId))
        .AppendUri("?{0}={1}", FileUriParameterName, fileUri)
        .AppendFormat("&{0}={1}", LocaleIdsParameterName, approvedLocales);
      
      var request = PreparePutRequest(uriBuilder.ToString(), auth.GetToken());
      ExecuteGetRequest(request, uriBuilder, auth);
    }

    public virtual void Unauthorize(string fileUri, string removedLocales)
    {
      var uriBuilder = this.GetRequestStringBuilder(ContentAuthorizationUrl.FormatUri(projectId))
        .AppendUri("?{0}={1}", FileUriParameterName, fileUri)
        .AppendFormat("&{0}={1}", LocaleIdsParameterName, removedLocales);

      var request = PrepareDeleteRequest(uriBuilder.ToString(), auth.GetToken());
      ExecuteGetRequest(request, uriBuilder, auth);
    }

    public virtual void DeleteFile(string fileUri)
    {
      var uriBuilder = this.GetRequestStringBuilder(DeleteUrl.FormatUri(projectId))
        .AppendUri("?{0}={1}", FileUriParameterName, fileUri);

      var request = PrepareDeleteRequest(uriBuilder.ToString(), auth.GetToken());
      ExecuteGetRequest(request, uriBuilder, auth);
    }

    public virtual Stream GetFileStream(string fileUri, string locale)
    {
      var uriBuilder = this.GetRequestStringBuilder(GetUrl.FormatUri(projectId, locale))
        .AppendUri("?{0}={1}", FileUriParameterName,fileUri);

      return GetResponseStream(PrepareGetRequest(uriBuilder.ToString(), auth.GetToken()));
    }
  }
}
