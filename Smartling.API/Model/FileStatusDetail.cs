using System;

namespace Smartling.Api.Model
{
  public class FileStatusDetail
  {
    public string fileUri;

    public int totalStringCount;

    public int totalWordCount;

    public int authorizedStringCount;

    public int completedStringCount;

    public int excludedStringCount;

    public int completedWordCount;

    public int authorizedWordCount;

    public int excludedWordCount;

    public DateTime lastUploaded;

    public string fileType;

    public string parserVersion;

    public bool hasInstructions;

    public string callbackUrl;
  }
}
