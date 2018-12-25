using Smartling.Api.Extensions;
using System;

namespace Smartling.Api.Model
{
  public class AuditLog
  {
    public AuditLog()
    {
    }

    public AuditLog(ActionType action_type, string user_id, string item_id, string file_uri, string path, string job_name, int? source_version, int? target_version, string source_locale_id, string target_locale_id)
    {
      this.action_type = action_type.GetName();
      this.user_id = user_id;
      this.item_id = item_id;
      this.file_uri = file_uri;
      this.path = path;
      this.job_name = job_name;
      this.source_version = source_version;
      this.target_version = target_version;
      this.source_locale_id = source_locale_id;
      this.target_locale_id = target_locale_id;
      this.time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
    }

    public string action_type { get; set; }
    public string user_id { get; set; }
    public string time { get; set; }
    public string description { get; set; }
    public string item_id { get; set; }
    public string path { get; set; }
    public string file_uri { get; set; }
    public string translation_job_uid { get; set; }
    public string job_name { get; set; }
    public string source_database { get; set; }
    public string source_locale_id { get; set; }
    public int? source_version { get; set; }
    public string target_locale_id { get; set; }
    public int? target_version { get; set; }
  }
}