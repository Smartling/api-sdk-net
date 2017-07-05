using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class JobRequest
  {
    public string jobName { get; set; }
    public List<string> targetLocaleIds { get; set; }
    public string description { get; set; }
    public string dueDate { get; set; }
    public string referenceNumber { get; set; }
    public string callbackUrl { get; set; }
    public string callbackMethod { get; set; }
  }
}
