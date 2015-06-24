using Newtonsoft.Json.Linq;

namespace PactNet.Matchers
{
    internal interface IMatcher
    {
        MatcherResult Match(string path, JToken expected, JToken actual);
    }
}