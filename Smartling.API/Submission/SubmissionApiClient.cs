using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Smartling.Api.Authentication;
using Smartling.Api.Model;

namespace Smartling.Api.Job
{
  public class SubmissionApiClient : ApiClientBase
  {
    private readonly string CreateSubmissionUrl = "/submission-service-api/v2/projects/{0}/buckets/{1}/translation-requests";
    private readonly string GetSubmissionsUrl = "/submission-service-api/v2/projects/{0}/buckets/{1}/translation-requests?limit={2}&offset={3}";
    private readonly string UpdateSubmissionUrl = "/submission-service-api/v2/projects/{0}/buckets/{1}/translation-requests/{2}";

    private readonly string projectId;
    private readonly string bucketName;
    private readonly IAuthenticationStrategy auth;
    private const int PageSize = 500;

    public SubmissionApiClient(IAuthenticationStrategy auth, string projectId, string bucketName)
    {
      this.auth = auth;
      this.projectId = projectId;
      this.bucketName = bucketName;
    }

    public virtual TranslationRequest CreateTranslationRequest(CreateTranslationRequest submission)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(CreateSubmissionUrl, projectId, bucketName));
      var response = ExecutePostRequest(uriBuilder, submission, auth);
      return JsonConvert.DeserializeObject<TranslationRequest>(response["response"]["data"].ToString());
    }

    public virtual TranslationRequest CreateDetails(CreateSubmissionDetails submission)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(CreateSubmissionUrl, projectId, bucketName));
      var response = ExecutePostRequest(uriBuilder, submission, auth);
      return JsonConvert.DeserializeObject<TranslationRequest>(response["response"]["data"].ToString());
    }

    public virtual TranslationRequest UpdateTranslationRequest(UpdateTranslationRequest request, string translationRequestUid)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(UpdateSubmissionUrl, projectId, bucketName, translationRequestUid));
      var response = ExecutePutRequest(uriBuilder, request, auth);
      return JsonConvert.DeserializeObject<TranslationRequest>(response["response"]["data"].ToString());
    }
    
    public virtual List<TranslationRequest> Get(string fileUri)
    {
      var page = GetPage(fileUri, PageSize, 0);
      var results = new List<TranslationRequest>();
      var pageNumber = 0;
      results.AddRange(page.items);

      while (page.totalCount > results.Count && page.items.Count > 0)
      {
        pageNumber++;
        page = GetPage(fileUri, PageSize, PageSize * pageNumber);
        results.AddRange(page.items);
      }

      return results;
    }

    public virtual SubmissionItemList GetPage(string fileUri, int limit, int offset)
    {
      StringBuilder uriBuilder;
      uriBuilder = this.GetRequestStringBuilder(string.Format(GetSubmissionsUrl, projectId, bucketName, limit, offset));

      if (!string.IsNullOrEmpty(fileUri))
      {
        uriBuilder.Append("&fileUri=" + fileUri);
      }

      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteGetRequest(request, uriBuilder, auth);
      return JsonConvert.DeserializeObject<SubmissionItemList>(response["response"]["data"].ToString());
    }
  }
}
