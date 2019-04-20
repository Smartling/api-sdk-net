using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class UpdateTranslationRequest<TCustomRequest, TCustomSubmission>
  {
    public TCustomRequest customOriginalData { get; set; }
    public List<UpdateSubmissionRequest<TCustomSubmission>> translationSubmissions { get; set; }
  }
}
