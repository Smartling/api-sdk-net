using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smartling.Api.Authentication;
using Smartling.Api.Exceptions;
using Smartling.Api.Model;

namespace Smartling.Api.PublishedFiles
{
  public class PublishedFilesApiClient : ApiClientBase
  {
    private readonly string RecentlyPublishedUrl = "/published-files-api/v2/projects/{0}/files/list/recently-published?publishedAfter={1}";
    private readonly string FileUrisParameterName = "fileUris[]";
    private readonly string LocaleIdsParameterName = "localeIds[]";
    private readonly string LimitParameterName = "limit";
    private readonly string OffsetParameterName = "offset";
    private readonly string projectId;
    private readonly IAuthenticationStrategy auth;

    public PublishedFilesApiClient(IAuthenticationStrategy auth, string projectId)
    {
      this.auth = auth;
      this.projectId = projectId;
    }

    public virtual RecentlyPublished GetRecentlyPublished(RecentlyPublishedSearch recentlyPublishedSearch)
    {
      if (!recentlyPublishedSearch.IsValid())
      {
        throw new Exception("Invalid parameters: " + recentlyPublishedSearch.LastError);
      }

      var uriBuilder = this.GetRequestStringBuilder(string.Format(RecentlyPublishedUrl, projectId, recentlyPublishedSearch.PublishedAfter.ToString("s", System.Globalization.CultureInfo.InvariantCulture)));
      foreach (var fileUri in recentlyPublishedSearch.FileUris)
      {
        uriBuilder.AppendFormat("&{0}={1}", FileUrisParameterName, System.Net.WebUtility.UrlEncode(fileUri));
      }

      foreach (var localeId in recentlyPublishedSearch.LocaleIds)
      {
        uriBuilder.AppendFormat("&{0}={1}", LocaleIdsParameterName, localeId);
      }

      if (recentlyPublishedSearch.Limit > 0)
      {
        uriBuilder.AppendFormat("&{0}={1}", LimitParameterName, recentlyPublishedSearch.Limit);
      }

      if (recentlyPublishedSearch.Offset > 0)
      {
        uriBuilder.AppendFormat("&{0}={1}", OffsetParameterName, recentlyPublishedSearch.Offset);
      }

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

      return JsonConvert.DeserializeObject<RecentlyPublished>(response["response"]["data"].ToString());
    }
  }
}
