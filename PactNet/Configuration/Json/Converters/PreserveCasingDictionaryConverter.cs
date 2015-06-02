using System;
using System.Collections;
using System.Globalization;
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
                var dictionary = (IDictionary) value;

                writer.WriteStartObject();

                foreach (DictionaryEntry entry in dictionary)
                {
                    var key = Convert.ToString(entry.Key, CultureInfo.InvariantCulture);
                    writer.WritePropertyName(key);
                    serializer.Serialize(writer, entry.Value);
                }

                writer.WriteEndObject();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new InvalidOperationException();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary).IsAssignableFrom(objectType);
        }
    }
}