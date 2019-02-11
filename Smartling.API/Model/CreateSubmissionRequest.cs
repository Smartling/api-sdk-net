namespace Smartling.Api.Model
{
  public class CreateSubmissionRequest
  {
    public string targetLocaleId { get; set; }
    public string state { get; set; }
    public TargetAssetKey targetAssetKey { get; set; }
    public CustomSubmissionData customTranslationData { get; set; }
    public string submitterName { get; set; }
  }
}