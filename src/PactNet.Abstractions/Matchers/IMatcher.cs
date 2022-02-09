using Newtonsoft.Json;

namespace PactNet.Matchers
{
    /// <summary>
    /// Matcher
    /// </summary>
    public interface IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>
        [JsonProperty("pact:matcher:type")]
        string Type { get; }

        /// <summary>
        /// Matcher value
        /// </summary>
        [JsonProperty("value")]
        dynamic Value { get; }
    }
}
