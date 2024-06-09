using System;
using System.Text.Json.Serialization;

namespace PactNet.Matchers
{
    /// <summary>
    /// Match every element of a collection with a min and/or max size against an example matcher
    /// </summary>
    public class MinMaxTypeMatcher : IMatcher
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
        /// Minimum items
        /// </summary>
        [JsonPropertyName("min")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Min { get; set; }

        /// <summary>
        /// Maximum items
        /// </summary>
        [JsonPropertyName("max")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Max { get; set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="MinMaxTypeMatcher"/> class.
        /// </summary>
        /// <param name="example">Example matcher</param>
        /// <param name="min">Minimum collection size</param>
        /// <param name="max">Maximum collection size</param>
        public MinMaxTypeMatcher(dynamic example, int min = default, int max = default)
        {
            if (min == default && max == default)
            {
                throw new ArgumentException("You must specify at least one of min and max. Min must be > 0");
            }

            // TODO: Remove this temporary workaround once the core library starts wrapping examples in arrays like Ruby did
            this.Value = new[] { example };
            this.Min = min;
            this.Max = max;
        }
    }
}
