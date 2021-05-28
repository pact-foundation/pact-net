using Newtonsoft.Json;

namespace PactNet.Matchers
{
    public interface IMatcher
    {
        [JsonProperty("pact:matcher:type")]
        string Type { get; }

        [JsonProperty("value")]
        dynamic Value { get; }
    }
}