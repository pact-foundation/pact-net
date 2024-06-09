using System.Text.Json.Serialization;

namespace PactNet.Matchers
{
    /// <summary>
    /// Match a property by type
    /// </summary>
    public class TypeMatcher : IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>
        [JsonPropertyName("pact:matcher:type")]
        public string Type => "type";

        /// <summary>
        /// Matcher value
        /// </summary>
        [JsonPropertyName("value")]
        public dynamic Value { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="TypeMatcher"/> class.
        /// </summary>
        /// <param name="example">Example value</param>
        public TypeMatcher(dynamic example)
        {
            this.Value = example;
        }
    }
}
