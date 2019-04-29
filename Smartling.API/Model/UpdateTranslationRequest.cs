using Newtonsoft.Json;
using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class UpdateTranslationRequest<TCustomRequest, TTargetKey, TCustomSubmission>
  {
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TCustomRequest customOriginalData { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<UpdateSubmissionRequest<TTargetKey, TCustomSubmission>> translationSubmissions { get; set; }
  }
}
