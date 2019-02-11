using System;

namespace Smartling.Api.Model
{
  public class TranslationSubmission
  {
    public string translationSubmissionUid { get; set; }
    public string translationRequestUid { get; set; }
    public TargetAssetKey targetAssetKey { get; set; }
    public CustomSubmissionData customTranslationData { get; set; }
    public string targetLocaleId { get; set; }
    public string state { get; set; }
    public string submitterName { get; set; }
    public int percentComplete { get; set; }
    public object lastExportedDate { get; set; }
    public object lastErrorMessage { get; set; }

    public DateTime createdDate { get; set; }
    public DateTime modifiedDate { get; set; }
  }
}