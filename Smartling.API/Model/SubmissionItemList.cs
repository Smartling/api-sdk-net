using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class SubmissionItemList
  {
    public int totalCount { get; set; }
    public List<TranslationRequest> items { get; set; }
  }
}
