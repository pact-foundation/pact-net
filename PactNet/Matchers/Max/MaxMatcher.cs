using System;
using System.Linq;
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
            var act = actual as JArray;
            var matches = act != null && actual.Count() > this.MaxValue;

            return matches ?
                new MatcherResult(new SuccessfulMatcherCheck(path)) :
                new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.AdditionalItemInArray));
        }
    }
}