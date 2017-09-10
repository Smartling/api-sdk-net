namespace Smartling.Api.Model
{
  public class AddFileToJobResponse
  {
    public int successCount { get; set; }
    public int failCount { get; set; }
    public string message;
    public string url;
  }
}
