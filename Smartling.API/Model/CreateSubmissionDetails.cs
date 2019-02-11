using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class CreateSubmissionDetails
  {
    public string translationRequestUid { get; set; }
    public List<CreateSubmissionRequest> translationSubmissions { get; set; }
  }
}
