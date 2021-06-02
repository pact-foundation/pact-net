namespace PactNet.Matchers.Numeric
{
    /// <summary>
    /// Match an integer specifically
    /// </summary>
    public class IntegerMatcher : IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>
        public string Type => "integer";

        /// <summary>
        /// Matcher value
        /// </summary>
        public dynamic Value { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="IntegerMatcher"/> class.
        /// </summary>
        /// <param name="value">Example value</param>
        internal IntegerMatcher(int value)
        {
            this.Value = value;
        }
    }
}
