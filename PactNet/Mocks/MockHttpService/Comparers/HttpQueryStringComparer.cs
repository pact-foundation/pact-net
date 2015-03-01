using System;
using System.Linq;
using System.Text.RegularExpressions;
using Nancy.Helpers;
using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpQueryStringComparer : IHttpQueryStringComparer
    {
        private readonly string _messagePrefix;

        public HttpQueryStringComparer(string messagePrefix)
        {
            _messagePrefix = messagePrefix;
        }

        public ComparisonResult Compare(string expected, string actual)
        {
            if (String.IsNullOrEmpty(expected) && String.IsNullOrEmpty(actual))
            {
                return new ComparisonResult();
            }

            var normalisedExpectedQuery = NormaliseUrlEncodingAndTrimTrailingAmpersand(expected);
            var normalisedActualQuery = NormaliseUrlEncodingAndTrimTrailingAmpersand(actual);
            var result = new ComparisonResult("{0} has query set to {1}", _messagePrefix, normalisedExpectedQuery ?? "null");

            if (expected == null || actual == null)
            {
                result.RecordFailure(expected, actual);
                return result;
            }

            var expectedQueryItems = HttpUtility.ParseQueryString(normalisedExpectedQuery);
            var actualQueryItems = HttpUtility.ParseQueryString(normalisedActualQuery);

            if (expectedQueryItems.Count != actualQueryItems.Count)
            {
                result.RecordFailure(normalisedExpectedQuery, normalisedActualQuery);
                return result;
            }

            foreach (string expectedKey in expectedQueryItems)
            {
                if (!actualQueryItems.AllKeys.Contains(expectedKey))
                {
                    result.RecordFailure(normalisedExpectedQuery, normalisedActualQuery);
                    return result;
                }

                var expectedValue = expectedQueryItems[expectedKey];
                var actualValue = actualQueryItems[expectedKey];

                if (expectedValue != actualValue)
                {
                    result.RecordFailure(normalisedExpectedQuery, normalisedActualQuery);
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