namespace Smartling.Api.Model
{
  public class FileStatusItem
  {
    public string localeId;

    public int authorizedStringCount;

    public int completedStringCount;
    
    public int authorizedWordCount;

    public int excludedWordCount;

    public string parserVersion;

    public bool hasInstructions;
  }
}
