using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Equality
{
    public class EqualityMatcher : IMatcher
    {
        [JsonIgnore]
        public string Type
        {
            get { return EqualityMatchDefinition.Name; }
        }

        public EqualityMatcher()
        {
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            var act = actual as JValue;
            var exp = expected as JValue;

            var matches = act != null && exp.Value == act.Value;

            return matches ?
                new MatcherResult(new SuccessfulMatcherCheck(path)) :
                new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueNotEqual));
        }
    }
}