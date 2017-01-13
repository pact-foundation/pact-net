using System;
using System.Linq;
using System.Text.RegularExpressions;
using Nancy.Helpers;
using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpQueryStringComparer : IHttpQueryStringComparer
    {
        public ComparisonResult Compare(string expected, string actual)
        {
            if (string.IsNullOrEmpty(expected) && string.IsNullOrEmpty(actual))
            {
                return new ComparisonResult("has no query strings");
            }

            var normalisedExpectedQuery = NormaliseUrlEncodingAndTrimTrailingAmpersand(expected);
            var normalisedActualQuery = NormaliseUrlEncodingAndTrimTrailingAmpersand(actual);
            var result = new ComparisonResult("has query {0}", normalisedExpectedQuery ?? "null");

            if (expected == null || actual == null)
            {
                result.RecordFailure(new DiffComparisonFailure(expected, actual));
                return result;
            }

            var expectedQueryItems = HttpUtility.ParseQueryString(normalisedExpectedQuery);
            var actualQueryItems = HttpUtility.ParseQueryString(normalisedActualQuery);

            if (expectedQueryItems.Count != actualQueryItems.Count)
            {
                result.RecordFailure(new DiffComparisonFailure(normalisedExpectedQuery, normalisedActualQuery));
                return result;
            }

            foreach (string expectedKey in expectedQueryItems)
            {
                if (!actualQueryItems.AllKeys.Contains(expectedKey))
                {
                    result.RecordFailure(new DiffComparisonFailure(normalisedExpectedQuery, normalisedActualQuery));
                    return result;
                }

                var expectedValue = expectedQueryItems[expectedKey];
                var actualValue = actualQueryItems[expectedKey];

                if (expectedValue != actualValue)
                {
                    result.RecordFailure(new DiffComparisonFailure(normalisedExpectedQuery, normalisedActualQuery));
                    return result;
                }
            }

            return result;
        }

        private string NormaliseUrlEncodingAndTrimTrailingAmpersand(string query)
        {
            return query != null ?
                Regex.Replace(query, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper()).TrimEnd('&') :
                query;
        }
    }
}