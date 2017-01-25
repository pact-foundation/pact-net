using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Integer
{
    public class IntegerMatcher : IMatcher
    {
        public string Type
        {
            get { return IntegerMatchDefinition.Name; }
        }

        public IntegerMatcher()
        {
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            var act = actual as JValue;
            var exp = expected as JValue;
            int intValue;

            var matches = act != null && int.TryParse(act.Value.ToString(), out intValue);

            return matches ?
                new MatcherResult(new SuccessfulMatcherCheck(path, "Integer", act.Value)) :
                new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueNotInteger, "Integer", act.Value));
        }
    }
}