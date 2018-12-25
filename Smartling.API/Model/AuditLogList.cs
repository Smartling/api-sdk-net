using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class AuditLogList
  {
    public int totalCount { get; set; }
    public List<AuditLog> items { get; set; }
  }
}
