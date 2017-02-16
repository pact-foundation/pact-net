using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Min
{
    public class MinMatcher : IMatcher
    {
        [JsonIgnore]
        public string Type
        {
            get { return MinMatchDefinition.Name; }
        }

        [JsonProperty("min")]
        public int MinValue { get; protected set; }

        public MinMatcher(int minValue)
        {
            MinValue = minValue;
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            var act = actual as JArray;

            if (act == null)
                return new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueDoesNotExist, this.MinValue, "(null)"));

            var matches = act.Count >= this.MinValue;

            return matches ?
                new MatcherResult(new SuccessfulMatcherCheck(path, this.MinValue, act.Count)) :
                new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.NotEnoughValuesInArray, this.MinValue, act.Count));
        }
    }
}