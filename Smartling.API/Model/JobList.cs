using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class JobList
  {
    public int totalCount { get; set; }
    public List<Job> items { get; set; }
  }
}
