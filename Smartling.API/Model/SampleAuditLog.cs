using System;

namespace Smartling.Api.Model
{
  public partial class SampleAuditLog : BaseAuditLog
  {
    public string action_type { get; set; }
    public string user_id { get; set; }
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

    public SampleAuditLog()
    {
    }

    public SampleAuditLog(SampleAuditLogBuilder builder)
    {
      this.action_type = builder.action_type;
      this.user_id = builder.user_id;
      this.item_id = builder.item_id;
      this.file_uri = builder.file_uri;
      this.path = builder.path;
      this.job_name = builder.translation_job_name;
      this.source_version = builder.source_version;
      this.target_version = builder.target_version;
      this.source_locale_id = builder.source_locale_id;
      this.target_locale_id = builder.target_locale_id;
      this.time = DateTime.UtcNow;
      this.bucket_name = builder.bucket_name;
    }
  }
}