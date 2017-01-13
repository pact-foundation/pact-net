using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PactNet.Configuration.Json.Converters
{
    public class PreserveCasingDictionaryConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                var dictionary = (IDictionary<string, string>) value;

                writer.WriteStartObject();

                foreach (var item in dictionary)
                {
                    writer.WritePropertyName(item.Key);
                    serializer.Serialize(writer, item.Value);
                }

                writer.WriteEndObject();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override bool CanRead => false;

        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary<string, string>).IsAssignableFrom(objectType);
        }
    }
}