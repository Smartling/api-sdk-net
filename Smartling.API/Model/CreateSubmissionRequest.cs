namespace Smartling.Api.Model
{
  public class CreateSubmissionRequest<T>
  {
    public string targetLocaleId { get; set; }
    public string state { get; set; }
    public TargetAssetKey targetAssetKey { get; set; }
    public T customTranslationData { get; set; }
    public string submitterName { get; set; }
  }
}