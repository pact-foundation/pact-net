namespace PactNet.Matchers
{
    /// <summary>
    /// Matcher that checks for explicit equality
    /// </summary>
    public class EqualityMatcher : IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>
        public string Type => "equality";

        /// <summary>
        /// Matcher value
        /// </summary>
        public dynamic Value { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="EqualityMatcher"/> class.
        /// </summary>
        /// <param name="value">Match value</param>
        internal EqualityMatcher(dynamic value)
        {
            this.Value = value;
        }
    }
}
