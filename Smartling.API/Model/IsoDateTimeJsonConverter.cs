using Newtonsoft.Json;
using System;

namespace Smartling.Api.Model
{
  public class IsoDateTimeJsonConverter : JsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var date = (DateTime)value;
      var isoDate = date.ToString("yyyy-MM-ddTHH:mm:ssZ");
      writer.WriteValue(isoDate);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
    }

    public override bool CanRead
    {
      get { return false; }
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(DateTime);
    }
  }
}
