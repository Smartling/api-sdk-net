namespace Smartling.Api.Model
{
  public class SampleAuditLogData
  {
    public string ItemId { get; set; }
    public string Path { get; set; }
    public string SourceDatabase { get; set; }
    public int? SourceVersion { get; set; }
    public int? TargetVersion { get; set; }
  }
}