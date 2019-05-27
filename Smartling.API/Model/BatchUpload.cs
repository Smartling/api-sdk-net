using System.Collections.Generic;
using System.IO;

namespace Smartling.Api.Model
{
  public class BatchUpload
  {
    public string FilePath { get; private set; }
    public string FileUri { get; private set; }
    public string FileType { get; private set; }
    public IEnumerable<string> ApprovedLocales { get; private set; }
    public string BatchUid { get; private set; }
    public string NameSpace { get; private set; }

    public sealed class Builder
    {
      private BatchUpload upload = new BatchUpload();

      public Builder CreateUpload(string filePath, string fileUri, string fileType, IEnumerable<string> approvedLocales, string batchUid)
      {
        upload.FilePath = filePath;
        upload.FileUri = fileUri;
        upload.FileType = fileType;
        upload.ApprovedLocales = approvedLocales;
        upload.BatchUid = batchUid;
        return this;
      }

      public Builder WithNameSpace(string nameSpace)
      {
        upload.NameSpace = nameSpace;
        return this;
      }

      public BatchUpload Build()
      {
        return upload;
      }

      public static implicit operator BatchUpload(Builder builder)
      {
        return builder.Build();
      }
    }
  }
}
