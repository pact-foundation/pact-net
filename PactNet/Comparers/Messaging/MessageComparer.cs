using Newtonsoft.Json.Linq;
using PactNet.Matchers;
using PactNet.Models.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Comparers.Messaging
{
    internal class MessageComparer
    {
        internal ComparisonResult Compare(Message expected, dynamic actual)
        {
            var result = new ComparisonResult();

            if (expected == null)
            {
                return result;
            }

            if (expected != null && actual == null)
            {
                result.RecordFailure(new ErrorMessageComparisonFailure("Actual Body is null"));
                return result;
            }

            //Need to wrap in a body root so that the expected matchingRules path lines up
            var actualToken = new JObject();
            actualToken.Add("body", JToken.FromObject(actual));

            var expectedToken = JToken.FromObject(expected);

            MatcherResult matchingResults = expected.Body.Validate(actualToken);
           
            var comparisonFailures = new List<ComparisonFailure>();

          
            foreach(FailedMatcherCheck failedCheck in matchingResults.MatcherChecks.Where(x => x is FailedMatcherCheck).Cast< FailedMatcherCheck >())
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
            return result;
        }

        internal ComparisonResult Compare(dynamic expected, dynamic actual, IDictionary<string, IMatcher> matchingRules)
        {
            var result = new ComparisonResult();

            if (expected == null)
            {
                return result;
            }

            if (expected != null && actual == null)
            {
                result.RecordFailure(new ErrorMessageComparisonFailure("Actual Body is null"));
                return result;
            }

            var expectedToken = JToken.FromObject(expected);
            var actualToken = JToken.FromObject(actual);

            foreach (var rule in matchingRules)
            {
                MatcherResult matchResult = rule.Value.Match(rule.Key, expectedToken, actualToken);

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
