using System;

namespace PactNet.Matchers
{
    /// <summary>
    /// Matcher for date values
    /// </summary>
    public class TimeMatcher : IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>
        public string Type => "time";

        /// <summary>
        /// Matcher value
        /// </summary>
        public dynamic Value { get; }

        /// <summary>
        /// Time format
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="TimeMatcher"/> class.
        /// </summary>
        /// <param name="example">Example value</param>
        /// <param name="format">Time format</param>
        public TimeMatcher(DateTime example, string format)
        {
            this.Value = example.ToString(format);
            this.Format = format;
        }
    }
}
