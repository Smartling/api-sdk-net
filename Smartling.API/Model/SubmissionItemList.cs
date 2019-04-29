using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class SubmissionItemList<TOriginalKey, TCustomRequest, TTargetKey, TCustomSubmission>
  {
    public int totalCount { get; set; }
    public List<TranslationRequest<TOriginalKey, TCustomRequest, TTargetKey, TCustomSubmission>> items { get; set; }
  }
}
