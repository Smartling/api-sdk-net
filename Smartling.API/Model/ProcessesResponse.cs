using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class ProcessItem
  {
    public string translationJobUid { get; set; }
  }

  public class ProcessesResponse
  {
    public List<ProcessItem> items { get; set; }
    public int totalCount { get; set; }
  }
}
