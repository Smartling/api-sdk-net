using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smartling.Api.Authentication;
using Smartling.Api.Exceptions;
using Smartling.Api.Model;
using System.Collections.Generic;
using System.Text;

namespace Smartling.Api.Project
{
  public class AuditApiClient : ApiClientBase
  {
    private readonly string CreateLogUrl = "/audit-log-api/v2/projects/{0}";
    private readonly string GetLogsUrl = "/audit-log-api/v2/projects/{0}/logs";

    private readonly string projectId;
    private readonly IAuthenticationStrategy auth;
    private const int PageSize = 500;

    public AuditApiClient(IAuthenticationStrategy auth, string projectId)
    {
      this.auth = auth;
      this.projectId = projectId;
    }

    public virtual AuditLog Create(AuditLog log)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(CreateLogUrl, projectId));
      var response = ExecutePostRequest(uriBuilder, log, auth);
      return JsonConvert.DeserializeObject<AuditLog>(response["response"]["data"].ToString());
    }

    public virtual List<AuditLog> Get()
    {
      var page = GetPage(PageSize, 0);
      var results = new List<AuditLog>();
      var pageNumber = 0;
      results.AddRange(page.items);

      while (page.totalCount > results.Count && page.items.Count > 0)
      {
        pageNumber++;
        page = GetPage(PageSize, PageSize * pageNumber);
        results.AddRange(page.items);
      }

      return results;
    }

    public virtual AuditLogList GetPage(int limit, int offset)
    {
      StringBuilder uriBuilder;
      uriBuilder = this.GetRequestStringBuilder(string.Format(GetLogsUrl, projectId, limit, offset));

      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteGetRequest(request, uriBuilder, auth);
      return JsonConvert.DeserializeObject<AuditLogList>(response["response"]["data"].ToString());
    }
  }
}
