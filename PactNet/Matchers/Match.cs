using PactNet.Mocks.MockHttpService.Matchers;
using PactNet.Mocks.MockHttpService.Matchers.Regex;
using PactNet.Mocks.MockHttpService.Matchers.Type;

namespace PactNet.Matchers
{
    public static class Match
    {
        public static Mocks.MockHttpService.Matchers.IMatcher Regex(dynamic example, string regex)
        {
            return new RegexMatcher(example, regex);
        }

        public static Mocks.MockHttpService.Matchers.IMatcher Type(dynamic example)
        {
            return new TypeMatcher(example);
        }

        public static Mocks.MockHttpService.Matchers.IMatcher MinType(dynamic example, int min)
        {
            return new MinType(example, min);
        }
    }
}