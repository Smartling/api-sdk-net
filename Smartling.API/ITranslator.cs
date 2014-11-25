using System.Collections.Generic;
using Smartling.Api.Model;

namespace Smartling.Api
{
  public interface ITranslator
  {
    FileStatus UploadFile(string filePath, string fileUri, string fileType, string approvedLocales, bool authorizeContent);
    string GetFile(string fileUri, string locale, string retrievalType);
    IEnumerable<FileStatus> GetFilesList(string locale);
    FileStatus GetFileStatus(string fileUri, string locale);
    void DeleteFile(string fileUri);
  }
}