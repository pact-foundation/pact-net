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
            long intValue;

            if (act == null)
                return new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueDoesNotExist, "Integer", "(null)"));

            if (act.Type != JTokenType.Integer)
                return new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueNotInteger, "Integer", string.Format("{0} ({1})", act.Value, act.Type)));

            var matches = act != null && long.TryParse(act.Value.ToString(), out intValue);

            return matches ?
                new MatcherResult(new SuccessfulMatcherCheck(path, "Integer", act.Value)) :
                new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueNotInteger, "Integer", act.Value));
        }
    }
}