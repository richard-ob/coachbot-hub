using Newtonsoft.Json;
using System;

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

    public class UlongNullableToStringConverter : JsonConverter<ulong?>
    {
        public override ulong? ReadJson(JsonReader reader, Type objectType, ulong? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }

            return Convert.ToUInt64(reader.Value);
        }

        public override void WriteJson(JsonWriter writer, ulong? value, JsonSerializer serializer)
        {
            if (value != null)
            {
                writer.WriteValue(value.ToString());
            }
        }
    }
}