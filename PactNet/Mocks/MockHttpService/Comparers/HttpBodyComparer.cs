using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using PactNet.Comparers;
using PactNet.Matchers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpBodyComparer : IHttpBodyComparer
    {
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

            JToken expectedToken = JToken.FromObject(expected);
            JToken actualToken = JToken.FromObject(actual);

            foreach (var rule in matchingRules)
            {
                var matchResult = rule.Match(expectedToken, actualToken);

                //TODO: Maybe we should call this a list of differences
                var comparisonFailures = new List<ComparisonFailure>();

                foreach (var failedCheck in matchResult.MatcherChecks.Where(x => x is FailedMatcherCheck).Cast<FailedMatcherCheck>())
                {
                    //TODO: We should be able to generate a better output, as we know exactly the path that failed
                    var comparisonFailure = new DiffComparisonFailure(expectedToken, actualToken);
                    if (comparisonFailures.All(x => x.Result != comparisonFailure.Result))
                    {
                        comparisonFailures.Add(comparisonFailure);
                    }
                }

                foreach (var failure in comparisonFailures)
                {
                    result.RecordFailure(failure);
                }

                //TODO: When more than 1 rule deal with the situation when a success overrides a failure (either more specific rule or order it's applied?)
            }

            return result;
        }
    }
}