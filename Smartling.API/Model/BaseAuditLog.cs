using Newtonsoft.Json;
using System;

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
    public DateTime time { get; set; }

    /// <summary>
    /// Virtual bucket name for your audit records
    /// </summary>
    /// <remarks>
    /// It can be useful if you use your integration from different environments but with
    /// the same project_uid. 
    /// </remarks>
    /// <example>
    /// Bucket name can be your env name. An example: stg; prod; local_dev_jack
    /// </example>
    public string bucket_name { get; set; }
  }
}
