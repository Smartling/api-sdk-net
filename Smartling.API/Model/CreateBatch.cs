using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class CreateBatch
  {
    public string translationJobUid { get; set; }
    public bool authorize { get; set; }
  }
}
