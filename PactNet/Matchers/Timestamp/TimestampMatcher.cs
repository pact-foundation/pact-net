using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Timestamp
{
    public class TimestampMatcher : IMatcher
    {
        [JsonIgnore]
        public string Type
        {
            get { return TimestampDefinition.Name; }
        }

        [JsonProperty("timestamp")]
        public string Format { get; protected set; }
        
        public TimestampMatcher(string format)
        {
            Format = format;
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            throw new NotImplementedException();
        }
    }
}