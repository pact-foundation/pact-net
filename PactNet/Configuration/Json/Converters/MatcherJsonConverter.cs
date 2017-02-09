using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using PactNet.Matchers;
using PactNet.Matchers.Date;
using PactNet.Matchers.Decimal;
using PactNet.Matchers.Equality;
using PactNet.Matchers.Integer;
using PactNet.Matchers.Max;
using PactNet.Matchers.Min;
using PactNet.Matchers.Regex;
using PactNet.Matchers.Timestamp;
using PactNet.Matchers.Type;
using PactNet.Models.Messaging;

namespace PactNet.Configuration.Json.Converters
{
    public class MatcherJsonConverter : JsonConverter
    {
        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var matcher = token.First as JProperty;

            if (matcher == null)
                throw new InvalidOperationException("Could not deserialize Matcher - invalid format");

            var result = default(IMatcher);

            //TODO: This assumes the matcher's first element. 
            //TODO: Probably should use something more reliable as it will likely break when refactoring the matcher serialization
            switch (matcher.Name)
            {
                case "match":
                    switch (matcher.First.Value<string>())
                    {
                        case "type":
                            result = new TypeMatcher();
                            break;
                        case "equality":
                            result = new EqualityMatcher();
                            break;
                        case "decimal":
                            result = new DecimalMatcher();
                            break;
                        case "integer":
                            result = new IntegerMatcher();
                            break;
                        default:
                            throw new InvalidOperationException(string.Format("Invalid matcher {0}", matcher.First.Value<string>()));
                    }
                    break;
                case "regex":
                    result = new RegexMatcher(matcher.First.Value<string>());
                    break;
                case "date":
                    result = new DateFormatMatcher(matcher.First.Value<string>());
                    break;
                case "timestamp":
                    result = new TimestampMatcher(matcher.First.Value<string>());
                    break;
                case "min":
                    result = new MinMatcher(matcher.First.Value<int>());
                    break;
                case "max":
                    result = new MaxMatcher(matcher.First.Value<int>());
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Invalid matcher identifier {0}", matcher.Name));
            }

            serializer.Populate(token.CreateReader(), result);
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IMatcher);
        }
    }
}
