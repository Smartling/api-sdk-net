using Newtonsoft.Json;
using System;

namespace Smartling.Api.Model
{
  public class UpdateSubmissionRequest<T>
  {
    public string translationSubmissionUid { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public TargetAssetKey targetAssetKey { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public T customTranslationData { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string targetLocaleId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string state { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string submitterName { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int percentComplete { get; set; }

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
