using System.Collections.Generic;

namespace PactNet.Matchers
{
    public class MatcherResult
    {
        public IEnumerable<MatcherCheck> MatcherChecks { get; private set; }

        public MatcherResult(MatcherCheck matcherCheck)
        {
            MatcherChecks = new List<MatcherCheck> { matcherCheck };
        }

        public MatcherResult(IEnumerable<MatcherCheck> matcherChecks)
        {
            MatcherChecks = matcherChecks;
        }
    }
}