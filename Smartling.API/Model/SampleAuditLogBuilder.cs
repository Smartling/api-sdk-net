namespace Smartling.Api.Model
{
  public class SampleAuditLogBuilder
  {
    public string bucket_name { get; set; }
    public string action_type { get; set; }
    public string user_id { get; set; }
    public string description { get; set; }
    public string item_id { get; set; }
    public string path { get; set; }
    public string file_uri { get; set; }
    public string translation_job_uid { get; set; }
    public string translation_job_name { get; set; }
    public string source_database { get; set; }
    public string source_locale_id { get; set; }
    public int? source_version { get; set; }
    public string target_locale_id { get; set; }
    public int? target_version { get; set; }

    public SampleAuditLogBuilder(string bucket_name, string action_type, string user_id, string item_id, string file_uri, string path)
    {
      this.bucket_name = bucket_name;
      this.action_type = action_type;
      this.user_id = user_id;
      this.item_id = item_id;
      this.file_uri = file_uri;
      this.path = path;
    }

    public SampleAuditLogBuilder WithJob(string translation_job_name, string translation_job_uid)
    {
      this.translation_job_name = translation_job_name;
      this.translation_job_uid = translation_job_uid;
      return this;
    }

    public SampleAuditLogBuilder WithSourceVersion(int source_version)
    {
      this.source_version = source_version;
      return this;
    }

    public SampleAuditLogBuilder WithTargetVersion(int target_version)
    {
      this.target_version = target_version;
      return this;
    }

    public SampleAuditLogBuilder WithSourceLocale(string source_locale_id)
    {
      this.source_locale_id = source_locale_id;
      return this;
    }

    public SampleAuditLogBuilder WithTargetLocale(string target_locale_id)
    {
      this.target_locale_id = target_locale_id;
      return this;
    }

    public static implicit operator SampleAuditLog(SampleAuditLogBuilder builder)
    {
      return new SampleAuditLog(builder);
    }
  }
}