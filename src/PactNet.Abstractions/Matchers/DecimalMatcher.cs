using System.Text.Json.Serialization;

namespace PactNet.Matchers
{
    /// <summary>
    /// Matcher for decimal values (i.e. numbers with a fractional component)
    /// </summary>
    public class DecimalMatcher : IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>
        [JsonPropertyName("pact:matcher:type")]
        public string Type => "decimal";

        /// <summary>
        /// Matcher value
        /// </summary>
        [JsonPropertyName("value")]
        public dynamic Value { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="DecimalMatcher"/> class.
        /// </summary>
        /// <param name="example">Example value</param>
        internal DecimalMatcher(double example)
        {
            this.Value = example;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DecimalMatcher"/> class.
        /// </summary>
        /// <param name="example">Example value</param>
        internal DecimalMatcher(float example)
        {
            this.Value = example;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="DecimalMatcher"/> class.
        /// </summary>
        /// <param name="example">Example value</param>
        internal DecimalMatcher(decimal example)
        {
            this.Value = example;
        }
    }
}
