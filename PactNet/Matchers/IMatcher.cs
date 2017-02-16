using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Configuration.Json.Converters;

namespace PactNet.Matchers
{
    [JsonConverter(typeof(MatcherJsonConverter))]
    public interface IMatcher
    {
        [JsonProperty("match")]
        string Type { get; }

        MatcherResult Match(string path, JToken expected, JToken actual);
    }
}