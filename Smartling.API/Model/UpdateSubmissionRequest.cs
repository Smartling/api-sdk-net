using Newtonsoft.Json;
using System;

namespace Smartling.Api.Model
{
  public class UpdateSubmissionRequest<TTargetKey, TCustomSubmission>
  {
    public string translationSubmissionUid { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TTargetKey targetAssetKey { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TCustomSubmission customTranslationData { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string targetLocaleId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string state { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTime? submittedDate { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string submitterName { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(IsoDateTimeJsonConverter))]
    public DateTime? lastExportedDate { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string lastErrorMessage { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof(IsoDateTimeJsonConverter))]
    public DateTime? modifiedDate { get; set; }
  }
}
