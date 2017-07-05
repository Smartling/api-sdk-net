using System.Collections.Generic;
using Newtonsoft.Json;
using Smartling.Api.Authentication;
using Smartling.Api.Model;

namespace Smartling.Api.Job
{
  public class JobApiClient : ApiClientBase
  {
    private readonly string JobUrl = "/jobs-api/v2/projects/{0}/jobs";
    private readonly string JobFileUrl = "/jobs-api/v2/projects/{0}/jobs/{1}/file/add";
    private readonly string JobAuthorizeUrl = "/jobs-api/v2/projects/{0}/jobs/{1}/authorize";
     
    private readonly string projectId;
    private readonly IAuthenticationStrategy auth;

    public JobApiClient(IAuthenticationStrategy auth, string projectId)
    {
      this.auth = auth;
      this.projectId = projectId;
    }

    public virtual Model.Job Create(JobRequest job)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(JobUrl, projectId));
      var response = ExecutePostRequest(uriBuilder, job, auth);
      return JsonConvert.DeserializeObject<Model.Job>(response["response"]["data"].ToString());
    }

    public virtual AddFileToJobResponse AddFile(string jobId, string fileUri)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(JobFileUrl, projectId, jobId));
      var response = ExecutePostRequest(uriBuilder, new { fileUri }, auth);
      return JsonConvert.DeserializeObject<AddFileToJobResponse>(response["response"]["data"].ToString());
    }

    public virtual AddFileToJobResponse Authorize(string jobId, IEnumerable<LocaleWorkflow> locales)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(JobAuthorizeUrl, projectId, jobId));
      var response = ExecutePostRequest(uriBuilder, new { localeWorkflows = locales }, auth);
      return JsonConvert.DeserializeObject<AddFileToJobResponse>(response["response"]["data"].ToString());
    }

    public virtual JobList GetAll()
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(JobUrl, projectId));
      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteGetRequest(request, uriBuilder, auth);
      return JsonConvert.DeserializeObject<JobList>(response["response"]["data"].ToString());
    }
  }
}
