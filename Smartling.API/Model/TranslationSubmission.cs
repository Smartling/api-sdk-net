using System;

namespace Smartling.Api.Model
{
  public class TranslationSubmission<T>
  {
    public string translationSubmissionUid { get; set; }
    public string translationRequestUid { get; set; }
    public TargetAssetKey targetAssetKey { get; set; }
    public T customTranslationData { get; set; }
    public string targetLocaleId { get; set; }
    public string state { get; set; }
    public string submitterName { get; set; }
    public int percentComplete { get; set; }
    public DateTime? lastExportedDate { get; set; }
    public string lastErrorMessage { get; set; }
    public DateTime createdDate { get; set; }
    public DateTime modifiedDate { get; set; }
  }
}