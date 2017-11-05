namespace Smartling.Api.Model
{
  // Subitem in collection of
  // /files-api/v2/projects/{0}/file/status
  public class FileStatusItem
  {
    public string localeId;

    public int authorizedStringCount;

    public int authorizedWordCount;

    public int completedStringCount;

    public int completedWordCount;
    
    public int excludedStringCount;

    public int excludedWordCount;
    
    public int totalStringCount;
  }
}
