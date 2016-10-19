using System;

namespace Smartling.Api.Model
{
  // Response for status for all locales
  // /files-api/v2/projects/{0}/file/status
  public class FileStatus
  {
    public string fileUri;

    public int totalStringCount;

    public int totalWordCount;

    public string fileType;

    public DateTime lastUploaded;

    public int totalCount;

    public FileStatusItem[] items;

    public string hasInstructions;
  }
}
