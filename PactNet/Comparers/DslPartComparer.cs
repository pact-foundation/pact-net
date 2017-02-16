using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using PactNet.Matchers;
using PactNet.Models.Consumer.Dsl;

namespace PactNet.Comparers
{
    internal class DslPartComparer : IDslPartComparer
    {
        public ComparisonResult Compare(DslPart expected, JToken actual)
        {
            var result = new ComparisonResult();

            MatcherResult matchingResults = expected.Validate(actual);

            foreach (FailedMatcherCheck failedCheck in matchingResults.MatcherChecks.Where(x => x is FailedMatcherCheck).Cast<FailedMatcherCheck>())
            {
                result.RecordFailure(new DiffComparisonFailure(failedCheck.ToString()));
            }

            return result;
        }
    }
}
