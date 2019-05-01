using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Smartling.Api.Model
{
  public class BaseAuditLog
  {
    /// <summary>
    /// Datetime when event happened in "yyyy-MM-ddTHH:i:sZ" format.
    /// It's mandatory field. Good idea to use UTC time
    /// </summary>
    /// <example>
    /// Time should look like "2018-12-28T13:51:13Z"
    /// <code>
    /// DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
    /// </code>
    /// </example>
    [JsonConverter(typeof(IsoDateTimeJsonConverter))]
    public DateTime actionTime { get; set; }

    /// <summary>
    ///  Event types. Possible events are: "UPLOAD", "DOWNLOAD", etc.
    /// </summary>
    public string actionType { get; set; }

    /// <summary>
    /// Virtual bucket name for your audit records
    /// </summary>
    /// <remarks>
    /// It can be useful if you use your integration from different environments but with
    /// the same project_uid. 
    /// </remarks>
    /// <example>
    /// envId name can be your env name. An example: stg; prod; local_dev_jack
    /// </example>
    public string envId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string fileUri { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string translationJobUid { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string translationJobName { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string translationJobDueDate { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool translationJobAuthorize { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string batchUid { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string sourceLocaleId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> targetLocaleIds { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string description { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string clientUserId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string clientUserEmail { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string clientUserName { get; set; }
  }
}
