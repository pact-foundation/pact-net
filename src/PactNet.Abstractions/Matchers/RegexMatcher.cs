using System.Text.Json.Serialization;

namespace PactNet.Matchers
{
    /// <summary>
    /// Match a string by regex
    /// </summary>
    public class RegexMatcher : IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>
        [JsonPropertyName("pact:matcher:type")]
        public string Type => "regex";

        /// <summary>
        /// Matcher value
        /// </summary>
        [JsonPropertyName("value")]
        public dynamic Value { get; }

        /// <summary>
        /// Matcher regex
        /// </summary>
        [JsonPropertyName("regex")]
        public string Regex { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="RegexMatcher"/> class.
        /// </summary>
        /// <param name="example">Example value</param>
        /// <param name="regex">Regex</param>
        internal RegexMatcher(string example, string regex)
        {
            this.Regex = regex;
            this.Value = example;
        }
    }
}
