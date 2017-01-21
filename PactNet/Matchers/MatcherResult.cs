using System.Collections.Generic;

namespace PactNet.Matchers
{
    public class MatcherResult
    {
        public List<MatcherCheck> MatcherChecks { get; private set; }

        public MatcherResult()
        {
            MatcherChecks = new List<MatcherCheck>();
        }

        public MatcherResult(MatcherCheck matcherCheck)
        {
            MatcherChecks = new List<MatcherCheck> { matcherCheck };
        }

        public MatcherResult(IEnumerable<MatcherCheck> matcherChecks)
        {
            MatcherChecks = new List<MatcherCheck>(matcherChecks);
        }

        public void Add(MatcherResult result)
        {
            this.MatcherChecks.AddRange(result.MatcherChecks);
        }
    }
}