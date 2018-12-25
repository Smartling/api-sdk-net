using Smartling.Api.Model;
using System.IO;

namespace Smartling.Api.Extensions
{
  public static class StringExtensions
  {
    public static Stream ToStream(this string s)
    {
      MemoryStream stream = new MemoryStream();
      StreamWriter writer = new StreamWriter(stream);
      writer.Write(s);
      writer.Flush();
      stream.Position = 0;
      return stream;
    }

    public static string GetName(this ActionType actionType)
    {
      switch (actionType)
      {
        case ActionType.Download:
          return "DOWNLOAD";
        case ActionType.Upload:
          return "UPLOAD";
      }

      return string.Empty;
    }
  }
}
