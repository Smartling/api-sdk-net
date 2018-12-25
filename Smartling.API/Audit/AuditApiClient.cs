﻿using Newtonsoft.Json;
using Smartling.Api.Authentication;
using Smartling.Api.Model;
using System.Collections.Generic;
using System.Text;

namespace Smartling.Api.Project
{
  public class AuditApiClient : ApiClientBase
  {
    private readonly string CreateLogUrl = "/audit-log-api/v2/projects/{0}/logs";
    private readonly string GetLogsUrl = "/audit-log-api/v2/projects/{0}/logs?limit={1}&offset={2}";

    private readonly string projectId;
    private readonly IAuthenticationStrategy auth;
    private const int PageSize = 500;

    public AuditApiClient(IAuthenticationStrategy auth, string projectId)
    {
      this.auth = auth;
      this.projectId = projectId;
    }

    public virtual void Create(AuditLog log)
    {
      var uriBuilder = this.GetRequestStringBuilder(string.Format(CreateLogUrl, projectId));
      var response = ExecutePostRequest(uriBuilder, log, auth);
    }

    public virtual List<AuditLog> Get(Dictionary<string, string> query, string sort)
    {
      var page = GetPage(query, sort, PageSize, 0);
      var results = new List<AuditLog>();
      var pageNumber = 0;
      results.AddRange(page.items);

      while (page.totalCount > results.Count && page.items.Count > 0)
      {
        pageNumber++;
        page = GetPage(query, sort, PageSize, PageSize * pageNumber);
        results.AddRange(page.items);
      }

      return results;
    }

    public virtual AuditLogList GetPage(Dictionary<string, string> query, string sort, int limit, int offset)
    {
      StringBuilder uriBuilder;
      uriBuilder = this.GetRequestStringBuilder(string.Format(GetLogsUrl, projectId, limit, offset));
      if(query != null)
      {
        uriBuilder.Append("&q=");
        foreach(var key in query.Keys)
        {
          uriBuilder.Append(key + ":" + query[key]);
        }
      }

      if (!string.IsNullOrEmpty(sort))
      {
        uriBuilder.Append("&sort=" + sort);
      }

      var request = PrepareGetRequest(uriBuilder.ToString(), auth.GetToken());
      var response = ExecuteGetRequest(request, uriBuilder, auth);
      return JsonConvert.DeserializeObject<AuditLogList>(response["response"]["data"].ToString());
    }
  }
}
