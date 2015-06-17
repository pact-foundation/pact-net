using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Comparers;
using PactNet.Matchers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpBodyComparer : IHttpBodyComparer
    {
        //TODO: Remove boolean and add "matching" functionality
        public ComparisonResult Compare(dynamic expected, dynamic actual, IEnumerable<IMatcher> matchingRules)
        {
            var result = new ComparisonResult("has a matching body");

            if (expected == null)
            {
                return result;
            }

            if (expected != null && actual == null)
            {
                result.RecordFailure(new ErrorMessageComparisonFailure("Actual Body is null"));
                return result;
            }

            foreach (var rule in matchingRules)
            {
                MatchResult matchResult = rule.Match(expected, actual);

                foreach (var failedChecks in matchResult.PerformedChecks.Where(x => x.Failed))
                {
                    result.RecordFailure(new ErrorMessageComparisonFailure(failedChecks.Message)); //TODO: This is hacky
                }

                //TODO: When more than 1 rule deal with the situation when a success overrides a failure (either more specific rule or order it's applied?)
            }

            return result;
        }
    }
}