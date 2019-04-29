namespace Smartling.Api.Model
{
  public class CreateSubmissionRequest<TTargetKey, TCustomSubmission>
  {
    public string targetLocaleId { get; set; }
    public string state { get; set; }
    public TTargetKey targetAssetKey { get; set; }
    public TCustomSubmission customTranslationData { get; set; }
    public string submitterName { get; set; }
  }
}