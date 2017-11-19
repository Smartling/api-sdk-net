using System.Collections.Generic;
using Newtonsoft.Json;
using Smartling.Api.Authentication;
using Smartling.Api.Model;

namespace Smartling.Api.Job
{
  public class JobApiClient : ApiClientBase
  {
    private readonly string CreateJobUrl = "/jobs-api/v3/projects/{0}/jobs";
    private readonly string UpdateJobUrl = "/jobs-api/v3/projects/{0}/jobs/{1}";
    private readonly string JobFileUrl = "/jobs-api/v3/projects/{0}/jobs/{1}/file/add";
    private readonly string JobAuthorizeUrl = "/jobs-api/v3/projects/{0}/jobs/{1}/authorize";
    private readonly string AddLocaleUrl = "/jobs-api/v3/projects/{0}/jobs/{1}/locales/{2}?syncContent={3}";
    private readonly string GetProcessesUrl = "/jobs-api/v3/projects/{0}/jobs/processes";

    private readonly string projectId;
    private readonly IAuthenticationStrategy auth;

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

    public virtual JobList GetAll()
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(CreateJobUrl, projectId));
      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteGetRequest(request, uriBuilder, auth);
      return JsonConvert.DeserializeObject<JobList>(response["response"]["data"].ToString());
    }
  }
}
