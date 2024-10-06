using System.Text.Json.Serialization;

namespace PactNet.Matchers
{
    public class ArrayContainsMatcher : IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>
        [JsonPropertyName("pact:matcher:type")]
        public string Type => "array-contains";

        /// <summary>
        /// The items expected to be in the array.
        /// </summary>
        [JsonPropertyName("variants")]
        public dynamic Value { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="ArrayContainsMatcher"/> class.
        /// </summary>
        /// <param name="variants"></param>
        public ArrayContainsMatcher(dynamic[] variants)
        {
            Value = variants;
        }
    }
}
