using Newtonsoft.Json.Converters;

namespace Smartling.Api.Model
{
  public class IsoDateTimeJsonConverter : IsoDateTimeConverter
  {
    public IsoDateTimeJsonConverter()
    {
      DateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";
    }
  }
}
