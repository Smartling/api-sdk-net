using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class UpdateTranslationRequest<T>
  {
    public List<UpdateSubmissionRequest<T>> translationSubmissions { get; set; }
  }
}
