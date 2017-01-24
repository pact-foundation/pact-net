using Newtonsoft.Json.Linq;

namespace PactNet.Matchers.Type
{
    public class TypeMatcher : IMatcher
    {
        public string Type
        {
            get { return TypeMatchDefinition.Name; }
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            var act = actual as JValue;
            var exp = expected as JValue;

            var matches = act != null && exp.Type == act.Type;

            return matches ?
                new MatcherResult(new SuccessfulMatcherCheck(path)) :
                new MatcherResult(new FailedMatcherCheck(path, MatcherCheckFailureType.ValueDoesNotMatch));
        }
    }
}