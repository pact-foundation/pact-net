using Newtonsoft.Json;

namespace PactNet.Matchers.Timestamp
{
    public class TimestampDefinition : MatchDefinition
    {
        public const string Name = "timestamp";

        [JsonProperty("timestamp")]
        public string Format { get; protected set; }

        public TimestampDefinition(object example, string format) :
            base(Name, example)
        {
            Format = format;
        }
    }
}