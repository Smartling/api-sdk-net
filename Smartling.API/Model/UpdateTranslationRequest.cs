using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class UpdateTranslationRequest
  {
    public List<UpdateSubmissionRequest> translationSubmissions { get; set; }
  }
}
