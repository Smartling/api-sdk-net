using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class AuditLogList<T>
  {
    public int totalCount { get; set; }
    public List<T> items { get; set; }
  }
}
