using System;

namespace Smartling.Api.Model
{
  // Response for status of single locale
  // /files-api/v2/projects/{0}/locales/{1}/file/status
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
