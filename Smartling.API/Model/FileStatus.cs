using System;

namespace Smartling.Api.Model
{
  public class FileStatus
  {
    public string fileUri;

    public int totalStringCount;

    public int totalWordCount;

    public string fileType;

    public DateTime lastUploaded;

    public int totalCount;

    public FileStatusDetail[] items;

    public string hasInstructions;
  }
}
