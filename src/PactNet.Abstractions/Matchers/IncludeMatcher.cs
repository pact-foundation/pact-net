using System.Text.Json.Serialization;

namespace PactNet.Matchers
{
    /// <summary>
    /// Matcher for a string include
    /// </summary>
    public class IncludeMatcher : IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>
        [JsonPropertyName("pact:matcher:type")]
        public string Type => "include";

        /// <summary>
        /// Matcher value
        /// </summary>
        [JsonPropertyName("value")]
        public dynamic Value { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="IncludeMatcher"/> class.
        /// </summary>
        /// <param name="include">Include string</param>
        internal IncludeMatcher(string include)
        {
            this.Value = include;
        }
    }
}
