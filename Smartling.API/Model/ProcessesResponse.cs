using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class ProcessesResponse
  {
    public InnerProcessesResponse response { get; set; }
  }

  public class ProcessItem
  {
    public string translationJobUid { get; set; }
  }

  public class ProcessesData
  {
    public List<ProcessItem> items { get; set; }
    public int totalCount { get; set; }
  }

  public class InnerProcessesResponse
  {
    public string code { get; set; }
    public ProcessesData data { get; set; }
  }
}
