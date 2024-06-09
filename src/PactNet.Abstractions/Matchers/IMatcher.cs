using System.Text.Json.Serialization;

namespace PactNet.Matchers
{
    /// <summary>
    /// Matcher
    /// </summary>
    [JsonConverter(typeof(MatcherConverter))]
    public interface IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>

        [JsonPropertyName("pact:matcher:type")]
        string Type { get; }

        /// <summary>
        /// Matcher value
        /// </summary>

        [JsonPropertyName("value")]
        dynamic Value { get; }
    }
}
