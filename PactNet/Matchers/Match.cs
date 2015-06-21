using PactNet.Mocks.MockHttpService.Matchers.Regex;
using PactNet.Mocks.MockHttpService.Matchers.Type;

namespace PactNet.Matchers
{
    public static class Match
    {
        public static MatchDefinition Regex(dynamic example, string regex)
        {
            return new RegexMatchDefinition(example, regex);
        }

        public static MatchDefinition Type(dynamic example)
        {
            return new TypeMatchDefinition(example);
        }
    }
}