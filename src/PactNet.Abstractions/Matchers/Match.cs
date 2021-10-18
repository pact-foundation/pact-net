namespace PactNet.Matchers
{
    public static class Match
    {
        /// <summary>
        /// Match a string property against a regex
        /// </summary>
        /// <param name="example">String example</param>
        /// <param name="regex">Match regex</param>
        /// <returns>Matcher</returns>
        public static IMatcher Regex(string example, string regex)
        {
            return new RegexMatcher(example, regex);
        }

        /// <summary>
        /// Match a property by type
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Type(dynamic example)
        {
            return new TypeMatcher(example);
        }

        /// <summary>
        /// Match every element of a collection with a min size against an example matcher
        /// </summary>
        /// <param name="example">Example to match each element against</param>
        /// <param name="min">Minimum collection size</param>
        /// <returns>Matcher</returns>
        public static IMatcher MinType(dynamic example, int min)
        {
            return new MinMaxTypeMatcher(example, min);
        }

        /// <summary>
        /// Match every element of a collection with a max size against an example matcher
        /// </summary>
        /// <param name="example">Example to match each element against</param>
        /// <param name="max">Maximum collection size</param>
        /// <returns>Matcher</returns>
        public static IMatcher MaxType(dynamic example, int max)
        {
            return new MinMaxTypeMatcher(example, max: max);
        }

        /// <summary>
        /// Match every element of a collection with a min and max size against an example matcher
        /// </summary>
        /// <param name="example">Example to match each element against</param>
        /// <param name="min">Minimum collection size</param>
        /// <param name="max">Maximum collection size</param>
        /// <returns>Matcher</returns>
        public static IMatcher MinMaxType(dynamic example, int min, int max)
        {
            return new MinMaxTypeMatcher(example, min, max);
        }

        /// <summary>
        /// Matcher which matches specifically on integers (i.e. not decimals)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Integer(int example)
        {
            return new IntegerMatcher(example);
        }

        /// <summary>
        /// Matcher which matches specifically on decimals (i.e. numbers with a fractional component)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Decimal(double example)
        {
            return new DecimalMatcher(example);
        }

        /// <summary>
        /// Matcher which matches specifically on decimals (i.e. numbers with a fractional component)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Decimal(float example)
        {
            return new DecimalMatcher(example);
        }

        /// <summary>
        /// Matcher which matches specifically on decimals (i.e. numbers with a fractional component)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Decimal(decimal example)
        {
            return new DecimalMatcher(example);
        }

        /// <summary>
        /// Matcher which matches specifically any numeric type (fractional or not)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Number(int example)
        {
            return new NumericMatcher(example);
        }

        /// <summary>
        /// Matcher which matches specifically any numeric type (fractional or not)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Number(double example)
        {
            return new NumericMatcher(example);
        }

        /// <summary>
        /// Matcher which matches specifically any numeric type (fractional or not)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Number(float example)
        {
            return new NumericMatcher(example);
        }

        /// <summary>
        /// Matcher which matches specifically any numeric type (fractional or not)
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Number(decimal example)
        {
            return new NumericMatcher(example);
        }

        /// <summary>
        /// Matcher which matches an exact value
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Equality(dynamic example)
        {
            return new EqualityMatcher(example);
        }

        /// <summary>
        /// Matcher which matches an explicit null value
        /// </summary>
        /// <returns>Matcher</returns>
        public static IMatcher Null()
        {
            return new NullMatcher();
        }

        /// <summary>
        /// Matcher which checks that a string property includes an example string
        /// </summary>
        /// <param name="example">Example value</param>
        /// <returns>Matcher</returns>
        public static IMatcher Include(string example)
        {
            return new IncludeMatcher(example);
        }
    }
}
