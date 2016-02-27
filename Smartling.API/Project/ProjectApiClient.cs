using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smartling.Api.Authentication;
using Smartling.Api.Exceptions;

namespace Smartling.Api.Project
{
  public class ProjectApiClient : ApiClientBase
  {
    private readonly string ProjectUrl = "/projects-api/v2/projects/{0}";

    private readonly string projectId;
    private readonly IAuthenticationStrategy auth;

    public ProjectApiClient(IAuthenticationStrategy auth, string projectId)
    {
      this.auth = auth;
      this.projectId = projectId;
    }
    
    public virtual ProjectData GetProjectData()
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(ProjectUrl, projectId));
      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
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

      return JsonConvert.DeserializeObject<ProjectData>(response["response"]["data"].ToString());
    }
  }
}
