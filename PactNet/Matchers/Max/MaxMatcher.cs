using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Max
{
    public class MaxMatcher : IMatcher
    {
        [JsonIgnore]
        public string Type
        {
            get { return MaxMatchDefinition.Name; }
        }

        [JsonProperty("max")]
        public int MaxValue { get; protected set; }

        public MaxMatcher(int maxValue)
        {
            MaxValue = maxValue;
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            throw new NotImplementedException();
        }
    }
}