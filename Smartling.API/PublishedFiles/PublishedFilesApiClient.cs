using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smartling.Api.Authentication;
using Smartling.Api.Exceptions;
using Smartling.Api.Model;
using System;
using System.Collections.Generic;

namespace Smartling.Api.Project
{
  public class PublishedFilesApiClient : ApiClientBase
  {
    private readonly string RecentlyPublishedUrl = "/published-files-api/v2/projects/{0}/files/list/recently-published?publishedAfter={1}";
    private readonly string projectId;
    private const int DefaultPeriod = 5;
    private readonly IAuthenticationStrategy auth;

    public PublishedFilesApiClient(IAuthenticationStrategy auth, string projectId)
    {
      this.auth = auth;
      this.projectId = projectId;
    }

    public virtual List<PublishedItem> GetRecentlyPublished()
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(RecentlyPublishedUrl, projectId, DateTime.UtcNow.AddDays(-DefaultPeriod).ToString("s", System.Globalization.CultureInfo.InvariantCulture)));
      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
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

      var result = JsonConvert.DeserializeObject<RecentlyPublished>(response["response"]["data"].ToString());
      return result.items ?? new List<PublishedItem>();
    }
  }
}
