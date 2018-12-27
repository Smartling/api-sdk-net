namespace Smartling.Api.Model
{
  public partial class AuditLog
  {
    public class AuditLogBuilder
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

      public AuditLogBuilder(string action_type, string user_id, string item_id, string file_uri, string path)
      {
        this.action_type = action_type;
        this.user_id = user_id;
        this.item_id = item_id;
        this.file_uri = file_uri;
        this.path = path;
      }

      public AuditLogBuilder WithJob(string job_name)
      {
        this.job_name = job_name;
        return this;
      }

      public AuditLogBuilder WithSourceVersion(int source_version)
      {
        this.source_version = source_version;
        return this;
      }

      public AuditLogBuilder WithTargetVersion(int target_version)
      {
        this.target_version = target_version;
        return this;
      }

      public AuditLogBuilder WithSourceLocale(string source_locale_id)
      {
        this.source_locale_id = source_locale_id;
        return this;
      }

      public AuditLogBuilder WithTargetLocale(string target_locale_id)
      {
        this.target_locale_id = target_locale_id;
        return this;
      }

      public static implicit operator AuditLog(AuditLogBuilder builder)
      {
        return new AuditLog(builder);
      }
    }
  }
}