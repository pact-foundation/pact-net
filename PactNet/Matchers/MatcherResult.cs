using System.Collections.Generic;

namespace PactNet.Matchers
{
    internal class MatcherResult
    {
        public IEnumerable<MatcherCheck> MatcherChecks { get; set; }
    }
}