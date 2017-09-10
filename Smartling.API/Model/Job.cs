using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class Job
  {
    public string translationJobUid { get; set; }
    public string jobName { get; set; }
    public List<string> targetLocaleIds { get; set; }
    public string description { get; set; }
    public string dueDate { get; set; }
    public string referenceNumber { get; set; }
    public string callbackUrl { get; set; }
    public string callbackMethod { get; set; }
    public string createdDate { get; set; }
    public string modifiedDate { get; set; }
    public string createdByUserUid { get; set; }
    public string modifiedByUserUid { get; set; }
    public string jobStatus { get; set; }
    public string firstCompletedDate { get; set; }
    public string lastCompletedDate { get; set; }
  }
}
