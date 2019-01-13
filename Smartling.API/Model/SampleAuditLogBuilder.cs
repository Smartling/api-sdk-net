namespace Smartling.Api.Model
{
  public class SampleAuditLogBuilder
  {
    public string actionType { get; set; }
    public string envId { get; set; }
    public string fileUri { get; set; }
    public string translationJobUid { get; set; }
    public string translationJobName { get; set; }
    public string translationJobDueDate { get; set; }
    public bool translationJobAutorize { get; set; }
    public string batchUid { get; set; }
    public string sourceLocaleId { get; set; }
    public string targetLocaleId { get; set; }
    public string targetLocaleIds { get; set; }
    public string description { get; set; }
    public string clientUserId { get; set; }
    public string clientUserEmail { get; set; }
    public string clientUserName { get; set; }
    public SampleAuditLogData clientData { get; set; }

    public SampleAuditLogBuilder(string envId, string actionType, string userId, string itemId, string fileUri, string path)
    {
      clientData = new SampleAuditLogData();
      this.envId = envId;
      this.actionType = actionType;
      this.clientUserId = userId;
      this.clientData.ItemId = itemId;
      this.fileUri = fileUri;
      this.clientData.Path = path;
    }

    public SampleAuditLogBuilder WithJob(string translationJobName, string translationJobUid, string batchUid)
    {
      this.translationJobName = translationJobName;
      this.translationJobUid = translationJobUid;
      this.batchUid = batchUid;
      return this;
    }

    public SampleAuditLogBuilder WithSourceVersion(int sourceVersion)
    {
      this.clientData.SourceVersion = sourceVersion;
      return this;
    }

    public SampleAuditLogBuilder WithTargetVersion(int targetVersion)
    {
      this.clientData.TargetVersion = targetVersion;
      return this;
    }

    public SampleAuditLogBuilder WithSourceLocale(string sourceLocaleId)
    {
      this.sourceLocaleId = sourceLocaleId;
      return this;
    }

    public SampleAuditLogBuilder WithTargetLocale(string targetLocaleId)
    {
      this.targetLocaleId = targetLocaleId;
      return this;
    }

    public static implicit operator SampleAuditLog(SampleAuditLogBuilder builder)
    {
      return new SampleAuditLog(builder);
    }
  }
}