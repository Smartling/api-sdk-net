namespace Smartling.Api.Model
{
  // Subitem in collection of
  // /files-api/v2/projects/{0}/file/status
  public class FileStatusItem
  {
    public string localeId;

    public int authorizedStringCount;

    public int completedStringCount;

    public int authorizedWordCount;

    public int excludedWordCount;

    public string parserVersion;

    public bool hasInstructions;

    public int totalStringCount;
  }
}
