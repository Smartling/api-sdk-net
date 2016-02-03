using System.Reflection;

namespace Smartling.Api.Model
{
  public class ClientUid
  {
    public string client { get; set; }
    public string version { get; set; }

    public static ClientUid DefaultUid()
    {
      return new ClientUid()
      {
        client = "smartling-api-sdk-net",
        version = Assembly.GetExecutingAssembly().GetName().Version.ToString()
      };
    }

    public string ToUserAgent()
    {
      return string.Format("{0}/{1}", client, version);
    }
  }
}
