using Smartling.Api.Model;
using System.IO;
using System.Linq;
using System.Text;

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

    public static string EscapeSearchQuery(this string query)
    {
      if (string.IsNullOrWhiteSpace(query)) return query;
      char[] special = { '+', '-', '=', '>', '<', '!', '(', ')', '{', '}', '[', ']', '^', '\"', '~', '*', '?', ':', '\\', '/', ' ' };
      char[] qArray = query.ToCharArray();

      var sb = new StringBuilder();
      foreach (var chr in qArray)
      {
        if (special.Contains(chr))
        {
          sb.Append(string.Format("\\{0}", chr));
        }
        else
        {
          sb.Append(chr);
        }
      }

      return sb.ToString();
    }
  }
}
