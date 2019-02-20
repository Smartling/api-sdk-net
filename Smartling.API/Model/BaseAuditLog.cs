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

    public string fileUri { get; set; }
    public string translationJobUid { get; set; }
    public string translationJobName { get; set; }
    public string translationJobDueDate { get; set; }
    public bool translationJobAuthorize { get; set; }
    public string batchUid { get; set; }
    public string sourceLocaleId { get; set; }
    public List<string> targetLocaleIds { get; set; }
    public string description { get; set; }

    public string clientUserId { get; set; }
    public string clientUserEmail { get; set; }
    public string clientUserName { get; set; }
  }
}
