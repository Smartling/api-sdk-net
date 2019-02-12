using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class SubmissionItemList<TCustomRequest, TCustomSubmission>
  {
    public int totalCount { get; set; }
    public List<TranslationRequest<TCustomRequest, TCustomSubmission>> items { get; set; }
  }
}
