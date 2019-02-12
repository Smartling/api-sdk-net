using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class CreateSubmissionDetails<T>
  {
    public string translationRequestUid { get; set; }
    public List<CreateSubmissionRequest<T>> translationSubmissions { get; set; }
  }
}
