namespace PactNet.Matchers.Numeric
{
    /// <summary>
    /// Matcher for any number
    /// </summary>
    public class NumericMatcher : IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>
        public string Type => "number";

        /// <summary>
        /// Matcher value
        /// </summary>
        public dynamic Value { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="NumericMatcher"/> class.
        /// </summary>
        /// <param name="example">Example value</param>
        internal NumericMatcher(double example)
        {
            this.Value = example;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="NumericMatcher"/> class.
        /// </summary>
        /// <param name="example">Example value</param>
        internal NumericMatcher(float example)
        {
            this.Value = example;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="NumericMatcher"/> class.
        /// </summary>
        /// <param name="example">Example value</param>
        internal NumericMatcher(decimal example)
        {
            this.Value = example;
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="NumericMatcher"/> class.
        /// </summary>
        /// <param name="example">Example value</param>
        internal NumericMatcher(int example)
        {
            this.Value = example;
        }
    }
}
