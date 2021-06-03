using Newtonsoft.Json;

namespace PactNet.Matchers
{
    /// <summary>
    /// Matcher against an explicit null value
    /// </summary>
    public class NullMatcher : IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>
        public string Type => "null";

        /// <summary>
        /// Matcher value
        /// </summary>
        [JsonIgnore]
        public dynamic Value => null;

        /// <summary>
        /// Initialises a new instance of the <see cref="NullMatcher"/> class.
        /// </summary>
        internal NullMatcher()
        {
        }
    }
}
