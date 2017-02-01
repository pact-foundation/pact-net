using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Equality
{
    public class EqualityMatcher : IMatcher
    {
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

            if (act == null)
                return new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueDoesNotExist, exp.Value, "(null)"));

            //Comparing string representation for equality to avoid numeric type variations
            var matches = string.CompareOrdinal(exp.Value<string>(), act.Value<string>()) == 0;

            return matches ?
                new MatcherResult(new SuccessfulMatcherCheck(path, exp.Value, act.Value)) :
                new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueNotEqual, exp.Value, act.Value));
        }
    }
}