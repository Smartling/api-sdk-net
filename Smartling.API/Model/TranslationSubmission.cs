using System;

namespace Smartling.Api.Model
{
  public class TranslationSubmission<TTargetKey, TCustomSubmission>
  {
    public string translationSubmissionUid { get; set; }
    public string translationRequestUid { get; set; }
    public TTargetKey targetAssetKey { get; set; }
    public TCustomSubmission customTranslationData { get; set; }
    public string targetLocaleId { get; set; }
    public string state { get; set; }
    public string submitterName { get; set; }
    public int percentComplete { get; set; }
    public DateTime? lastExportedDate { get; set; }
    public string lastErrorMessage { get; set; }
    public DateTime createdDate { get; set; }
    public DateTime modifiedDate { get; set; }
    public DateTime? submittedDate { get; set; }
  }
}