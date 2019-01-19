using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Smartling.Api.Authentication;
using Smartling.Api.Model;

namespace Smartling.Api.Job
{
  public class JobApiClient : ApiClientBase
  {
    private readonly string CreateJobUrl = "/jobs-api/v3/projects/{0}/jobs";
    private readonly string GetJobUrl = "/jobs-api/v3/projects/{0}/jobs?limit={1}&offset={2}";
    private readonly string GetJobByIdUrl = "/jobs-api/v3/projects/{0}/jobs/{1}";
    private readonly string GetJobByNameUrl = "/jobs-api/v3/projects/{0}/jobs?jobName={1}&limit={2}&offset={3}";
    private readonly string UpdateJobUrl = "/jobs-api/v3/projects/{0}/jobs/{1}";
    private readonly string JobFileUrl = "/jobs-api/v3/projects/{0}/jobs/{1}/file/add";
    private readonly string JobAuthorizeUrl = "/jobs-api/v3/projects/{0}/jobs/{1}/authorize";
    private readonly string AddLocaleUrl = "/jobs-api/v3/projects/{0}/jobs/{1}/locales/{2}?syncContent={3}";
    private readonly string GetProcessesUrl = "/jobs-api/v3/projects/{0}/jobs/processes";

    private readonly string projectId;
    private readonly IAuthenticationStrategy auth;
    private const int PageSize = 500;

    public JobApiClient(IAuthenticationStrategy auth, string projectId)
    {
      this.auth = auth;
      this.projectId = projectId;
    }

    public virtual Model.Job Create(CreateJob job)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(CreateJobUrl, projectId));
      var response = ExecutePostRequest(uriBuilder, job, auth);
      return JsonConvert.DeserializeObject<Model.Job>(response["response"]["data"].ToString());
    }

    public virtual Model.Job Update(UpdateJob job, string jobId)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(UpdateJobUrl, projectId, jobId));
      var response = ExecutePutRequest(uriBuilder, job, auth);
      return JsonConvert.DeserializeObject<Model.Job>(response["response"]["data"].ToString());
    }

    public virtual Model.Job GetById(string translationJobUid)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(GetJobByIdUrl, projectId, translationJobUid));     
      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteGetRequest(request, uriBuilder, auth);
      return JsonConvert.DeserializeObject<Model.Job>(response["response"]["data"].ToString());
    }

    public virtual AddFileToJobResponse AddFile(string jobId, string fileUri, List<string> targetLocaleIds = null)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(JobFileUrl, projectId, jobId));
      var response = ExecutePostRequest(uriBuilder, new { fileUri, targetLocaleIds }, auth);
      return JsonConvert.DeserializeObject<AddFileToJobResponse>(response["response"]["data"].ToString());
    }

    public virtual ProcessesResponse GetProcesses(string jobId)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(GetProcessesUrl, projectId, jobId));
      var response = ExecutePostRequest(uriBuilder, new GetProcesses() { translationJobUids = new []{jobId} }, auth);
      return JsonConvert.DeserializeObject<ProcessesResponse>(response["response"]["data"].ToString());
    }

    public virtual AddFileToJobResponse Authorize(string jobId)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(JobAuthorizeUrl, projectId, jobId));
      var response = ExecutePostRequest(uriBuilder, "{}", auth);
      return JsonConvert.DeserializeObject<AddFileToJobResponse>(response["response"]["data"].ToString());
    }

    public virtual AddFileToJobResponse AddLocale(string localeId, string jobId, bool syncContent = false)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(AddLocaleUrl, projectId, jobId, localeId, syncContent));
      var response = ExecutePostRequest(uriBuilder, null, auth);
      return JsonConvert.DeserializeObject<AddFileToJobResponse>(response["response"]["data"].ToString());
    }

    public virtual List<Smartling.Api.Model.Job> Get(string jobName = "")
    {
      var page = GetPage(jobName, PageSize, 0);
      var results = new List<Smartling.Api.Model.Job>();
      var pageNumber = 0;
      results.AddRange(page.items);

      while (page.totalCount > results.Count && page.items.Count > 0)
      {
        pageNumber++;
        page = GetPage(jobName, PageSize, PageSize * pageNumber);
        results.AddRange(page.items);
      }

      return results;
    }

    public virtual JobList GetPage(string jobName, int limit, int offset)
    {
      StringBuilder uriBuilder;
      if (string.IsNullOrEmpty(jobName))
      {
        uriBuilder = this.GetRequestStringBuilder(string.Format(GetJobUrl, projectId, limit, offset));
      }
      else
      {
        uriBuilder = this.GetRequestStringBuilder(string.Format(GetJobByNameUrl, projectId, jobName, limit, offset));
      }

      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteGetRequest(request, uriBuilder, auth);
      return JsonConvert.DeserializeObject<JobList>(response["response"]["data"].ToString());
    }
  }
}
