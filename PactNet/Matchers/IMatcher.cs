using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PactNet.Matchers
{
    public interface IMatcher
    {
        [JsonProperty("match")]
        string Type { get; }

        MatcherResult Match(string path, JToken expected, JToken actual);
    }
}