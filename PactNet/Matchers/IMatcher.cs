using Newtonsoft.Json.Linq;

namespace PactNet.Matchers
{
    internal interface IMatcher
    {
        string MatchPath { get; }
        MatcherResult Match(JToken expected, JToken actual);
    }
}