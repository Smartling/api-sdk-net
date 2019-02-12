namespace Smartling.Api.Model
{
  public class CreateTranslationRequest<T>
  {
    public OriginalAssetKey originalAssetKey { get; set; }
    public T customOriginalData { get; set; }
    public string title { get; set; }
    public string fileUri { get; set; }
    public string contentHash { get; set; }
    public string originalLocaleId { get; set; }
  }
}
