using Newtonsoft.Json;
using System;
using System.Globalization;

namespace CoachBot.Extensions
{
    public class UlongToStringConverter : JsonConverter<ulong>
    {
        public override ulong ReadJson(JsonReader reader, Type objectType, ulong existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return Convert.ToUInt64(reader.Value);
        }

        public override void WriteJson(JsonWriter writer, ulong value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
