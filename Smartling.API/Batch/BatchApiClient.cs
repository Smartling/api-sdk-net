using System.Collections.Specialized;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smartling.Api.Authentication;
using Smartling.Api.Exceptions;
using Smartling.Api.Model;

namespace Smartling.Api.Batch
{
  public class BatchApiClient : ApiClientBase
  {
    private readonly string CreateBatchUrl = "/jobs-batch-api/v1/projects/{0}/batches";
    private readonly string UploadBatchUrl = "/jobs-batch-api/v1/projects/{0}/batches/{1}/file";
    private readonly string ExecuteBatchUrl = "/jobs-batch-api/v1/projects/{0}/batches/{1}";

    private const string FileUriParameterName = "fileUri";
    private const string FileTypeParameterName = "fileType";
    private const string CallbackUrlParameterName = "callbackUrl";
    private const string LocalesToApproveParameterName = "localeIdsToAuthorize[]";
    private const string CliendUidParameterName = "smartling.client_lib_id";

    private readonly string projectId;
    private readonly string callbackUrl;
    private readonly IAuthenticationStrategy auth;

    public BatchApiClient(IAuthenticationStrategy auth, string projectId, string callbackUrl)
    {
      this.auth = auth;
      this.projectId = projectId;
      this.callbackUrl = callbackUrl;
    }

    public virtual Model.Batch Create(CreateBatch batch)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(CreateBatchUrl, projectId));
      var response = ExecutePostRequest(uriBuilder, batch, auth);
      return JsonConvert.DeserializeObject<Model.Batch>(response["response"]["data"].ToString());
    }

    public virtual BatchUploadResult UploadFile(string filePath, string fileUri, string fileType, string approvedLocales, bool authorizeContent, string batchUid)
    {
      using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
      {
        return UploadFileStream(fileStream, fileUri, fileType, approvedLocales, authorizeContent, batchUid);
      }
    }

    public virtual BatchUploadResult UploadFileStream(Stream fileStream, string fileUri, string fileType, string approvedLocales, bool authorizeContent, string batchUid)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(UploadBatchUrl, projectId, batchUid));
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

      return ExecuteUploadRequest(fileStream, fileUri, uriBuilder, formData);
    }

    private BatchUploadResult ExecuteUploadRequest(Stream fileStream, string fileUri, StringBuilder uriBuilder, NameValueCollection formData)
    {
      try
      {
        var request = PrepareFilePostRequest(uriBuilder.ToString(), fileUri, fileStream, formData, auth.GetToken());
        var response = JObject.Parse(GetResponse(request));
        return JsonConvert.DeserializeObject<BatchUploadResult>(response["response"]["data"].ToString());
      }
      catch (AuthenticationException)
      {
        var request = PrepareFilePostRequest(uriBuilder.ToString(), fileUri, fileStream, formData, auth.GetToken(true));
        var response = JObject.Parse(GetResponse(request));
        return JsonConvert.DeserializeObject<BatchUploadResult>(response["response"]["data"].ToString());
      }
    }
    
    public virtual void Execute(string batchUid)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(ExecuteBatchUrl, projectId, batchUid));
      ExecutePostRequest(uriBuilder, new ExecuteBatch() {action = "EXECUTE"}, auth);
    }
  }
}
