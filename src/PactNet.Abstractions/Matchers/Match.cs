using System;

namespace PactNet.Matchers
{
    /// <summary>
    /// Pact matcher
    /// </summary>
    public static class Match
    {
        /// <summary>
        /// Match a string property against a regex
        /// </summary>
        /// <param name="example">String example</param>
        /// <param name="regex">Match regex</param>
        /// <returns>Matcher</returns>
        public static IMatcher Regex(string example, string regex) => new RegexMatcher(example, regex);

        /// <summary>
        /// Match a property by type
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Type(dynamic example) => new TypeMatcher(example);

        /// <summary>
        /// Match every element of a collection with a min size against an example matcher
        /// </summary>
        /// <param name="example">Example to match each element against</param>
        /// <param name="min">Minimum collection size</param>
        /// <returns>Matcher</returns>
        public static IMatcher MinType(dynamic example, int min) => new MinMaxTypeMatcher(example, min);

        /// <summary>
        /// Match every element of a collection with a max size against an example matcher
        /// </summary>
        /// <param name="example">Example to match each element against</param>
        /// <param name="max">Maximum collection size</param>
        /// <returns>Matcher</returns>
        public static IMatcher MaxType(dynamic example, int max) => new MinMaxTypeMatcher(example, max: max);

        /// <summary>
        /// Match every element of a collection with a min and max size against an example matcher
        /// </summary>
        /// <param name="example">Example to match each element against</param>
        /// <param name="min">Minimum collection size</param>
        /// <param name="max">Maximum collection size</param>
        /// <returns>Matcher</returns>
        public static IMatcher MinMaxType(dynamic example, int min, int max) => new MinMaxTypeMatcher(example, min, max);

        /// <summary>
        /// Matcher which matches specifically on integers (i.e. not decimals)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Integer(int example) => new IntegerMatcher(example);

        /// <summary>
        /// Matcher which matches specifically on decimals (i.e. numbers with a fractional component)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Decimal(double example) => new DecimalMatcher(example);

        /// <summary>
        /// Matcher which matches specifically on decimals (i.e. numbers with a fractional component)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Decimal(float example) => new DecimalMatcher(example);

        /// <summary>
        /// Matcher which matches specifically on decimals (i.e. numbers with a fractional component)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Decimal(decimal example) => new DecimalMatcher(example);

        /// <summary>
        /// Matcher which matches specifically any numeric type (fractional or not)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Number(int example) => new NumericMatcher(example);

        /// <summary>
        /// Matcher which matches specifically any numeric type (fractional or not)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Number(double example) => new NumericMatcher(example);

        /// <summary>
        /// Matcher which matches specifically any numeric type (fractional or not)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Number(float example) => new NumericMatcher(example);

        /// <summary>
        /// Matcher which matches specifically any numeric type (fractional or not)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Number(decimal example) => new NumericMatcher(example);

        /// <summary>
        /// Matcher which matches an exact value
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Equality(dynamic example) => new EqualityMatcher(example);

        /// <summary>
        /// Matcher which matches an explicit null value
        /// </summary>
        /// <returns>Matcher</returns>
        public static IMatcher Null() => new NullMatcher();

        /// <summary>
        /// Matcher which checks that a string property includes an example string
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Include(string example) => new IncludeMatcher(example);

        /// <summary>
        /// Matcher which checks that a string property is in the given timestamp format
        /// </summary>
        /// <param name="timestamp">Timestamp value</param>
        /// <param name="format">Timestamp format</param>
        /// <returns>Matcher</returns>
        public static IMatcher Timestamp(DateTime timestamp, string format) => new DateTimeMatcher(timestamp, format);

        /// <summary>
        /// Matcher which checks that a string property is in ISO-8601 date format
        /// </summary>
        /// <param name="date">Date value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Date(DateTime date) => Date(date, "yyyy-MM-dd");

        /// <summary>
        /// Matcher which checks that a string property is in the given date format
        /// </summary>
        /// <param name="date">Date value</param>
        /// <param name="format">Date format</param>
        /// <returns>Matcher</returns>
        public static IMatcher Date(DateTime date, string format) => new DateMatcher(date, format);

        /// <summary>
        /// Matcher which checks that a string property is in ISO-8601 time format
        /// </summary>
        /// <param name="time">Time value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Time(DateTime time) => Time(time, "HH:mm:ss");

        /// <summary>
        /// Matcher which checks that a string property is in the given time format
        /// </summary>
        /// <param name="time">Time value</param>
        /// <param name="format">Time format</param>
        /// <returns>Matcher</returns>
        public static IMatcher Time(DateTime time, string format) => new TimeMatcher(time, format);
    }
}
