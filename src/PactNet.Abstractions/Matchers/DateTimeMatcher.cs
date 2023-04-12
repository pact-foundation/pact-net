using System;

namespace PactNet.Matchers
{
    /// <summary>
    /// Matcher for timestamp values (i.e. values with both date and time portions)
    /// </summary>
    public class DateTimeMatcher : IMatcher
    {
        /// <summary>
        /// Type of the matcher
        /// </summary>
        public string Type => "timestamp";

        /// <summary>
        /// Matcher value
        /// </summary>
        public dynamic Value { get; }

        /// <summary>
        /// Timestamp format
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="DateTimeMatcher"/> class.
        /// </summary>
        /// <param name="Example">Example value</param>
        /// <param name="format">Timestamp format</param>
        public DateTimeMatcher(DateTime Example, string format)
        {
            this.Value = Example.ToString(format);
            this.Format = format;
        }
    }
}
