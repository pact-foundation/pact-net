using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Decimal
{
    public class DecimalMatcher : IMatcher
    {
        public string Type
        {
            get { return DecimalMatchDefinition.Name; }
        }

        public DecimalMatcher()
        {
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            var act = actual as JValue;
            decimal decValue;

            var matches = act != null && decimal.TryParse(act.Value.ToString(), out decValue);

            return matches ?
                new MatcherResult(new SuccessfulMatcherCheck(path)) :
                new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueNotDecimal));
        }
    }
}