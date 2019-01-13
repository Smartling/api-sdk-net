using System;

namespace Smartling.Api.Model
{
  public partial class SampleAuditLog : BaseAuditLog
  {
    public SampleAuditLogData clientData { get; set; }

    public SampleAuditLog()
    {
      this.clientData = new SampleAuditLogData();
    }

    public SampleAuditLog(SampleAuditLogBuilder builder)
    {
      this.clientData = new SampleAuditLogData();
      this.actionType = builder.actionType;
      this.actionTime = DateTime.UtcNow;
      this.clientUserId = builder.clientUserId;
      this.fileUri = builder.fileUri;
      this.clientData.ItemId = builder.clientData.ItemId;
      this.clientData.Path = builder.clientData.Path;
      this.translationJobName = builder.translationJobName;
      this.clientData.SourceVersion = builder.clientData.SourceVersion;
      this.clientData.TargetVersion = builder.clientData.TargetVersion;
      this.clientData.SourceDatabase = builder.clientData.SourceDatabase;
      this.sourceLocaleId = builder.sourceLocaleId;
      this.targetLocaleIds = builder.targetLocaleIds;
      this.envId = builder.envId;
    }
  }
}