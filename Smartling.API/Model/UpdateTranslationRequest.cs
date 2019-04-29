using Newtonsoft.Json;
using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class UpdateTranslationRequest<TOriginalKey, TCustomRequest, TTargetKey, TCustomSubmission>
  {
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TOriginalKey originalAssetKey { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string originalLocale { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TCustomRequest customOriginalData { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<UpdateSubmissionRequest<TTargetKey, TCustomSubmission>> translationSubmissions { get; set; }
  }
}
