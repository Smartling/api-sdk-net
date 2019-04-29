using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class CreateSubmissionDetails<TTargetKey, TCustomSubmission>
  {
    public string translationRequestUid { get; set; }
    public List<CreateSubmissionRequest<TTargetKey, TCustomSubmission>> translationSubmissions { get; set; }
  }
}
