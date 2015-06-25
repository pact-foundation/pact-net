using Newtonsoft.Json.Linq;
using PactNet.Matchers;

namespace PactNet.Mocks.MockHttpService.Matchers.Type
{
    internal class TypeMatcher : IMatcher
    {
        public string Type
        {
            get { return TypeMatchDefinition.Name; }
        }

        public MatcherResult Match(string path, JToken expected, JToken actual)
        {
            return new MatcherResult(new SuccessfulMatcherCheck(path));
        }
    }
}