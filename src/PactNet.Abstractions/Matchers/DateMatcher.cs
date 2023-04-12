using System;

namespace PactNet.Matchers
{
    /// <summary>
    /// Matcher for date values
    /// </summary>
    public class DateMatcher : IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>
        public string Type => "date";

        /// <summary>
        /// Matcher value
        /// </summary>
        public dynamic Value { get; }

        /// <summary>
        /// Date format
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="DateMatcher"/> class.
        /// </summary>
        /// <param name="example">Example value</param>
        /// <param name="format">Date format</param>
        public DateMatcher(DateTime example, string format)
        {
            this.Value = example.ToString(format);
            this.Format = format;
        }
    }
}
