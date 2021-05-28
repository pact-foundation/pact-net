using PactNet.Matchers.Regex;
using PactNet.Matchers.Type;

namespace PactNet.Matchers
{
    public static class Match
    {
        public static IMatcher Regex(string example, string regex)
        {
            return new RegexMatcher(example, regex);
        }

        public static IMatcher Type(dynamic example)
        {
            return new TypeMatcher(example);
        }

        public static IMatcher MinType(dynamic example, int min)
        {
            return new MinTypeMatcher(example, min);
        }
    }
}