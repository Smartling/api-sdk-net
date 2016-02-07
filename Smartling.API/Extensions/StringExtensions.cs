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
  }
}
