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

            if (act == null)
                return new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueDoesNotExist, this.MaxValue, "(null)"));

            var matches = act.Count <= this.MaxValue;

            return matches ?
                new MatcherResult(new SuccessfulMatcherCheck(path, this.MaxValue, act.Count)) :
                new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.AdditionalItemInArray, this.MaxValue, act.Count));
        }
    }
}