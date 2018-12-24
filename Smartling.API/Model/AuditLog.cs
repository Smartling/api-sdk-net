using System;

namespace Smartling.Api.Model
{
  public class AuditLog
  {
    public ActionType action_type { get; set; }
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
    public int source_version { get; set; }
    public string target_locale_id { get; set; }
    public int target_version { get; set; }
  }
}