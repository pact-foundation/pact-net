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
            var result = new ComparisonResult();

            if (String.IsNullOrEmpty(expected) && String.IsNullOrEmpty(actual))
            {
                return result;
            }

            var normalisedExpectedQuery = NormaliseUrlEncodingAndTrimTrailingAmpersand(expected);
            var normalisedActualQuery = NormaliseUrlEncodingAndTrimTrailingAmpersand(actual);

            result.AddInfo(String.Format("{0} has query set to {1}", _messagePrefix, normalisedExpectedQuery ?? "null"));

            if (expected == null)
            {
                result.AddError(actual: actual);
                return result;
            }

            if (actual == null)
            {
                result.AddError(expected: expected);
                return result;
            }

            var expectedQueryItems = HttpUtility.ParseQueryString(normalisedExpectedQuery);
            var actualQueryItems = HttpUtility.ParseQueryString(normalisedActualQuery);

            if (expectedQueryItems.Count != actualQueryItems.Count)
            {
                result.AddError(expected: normalisedExpectedQuery, actual: normalisedActualQuery);
                return result;
            }

            foreach (string expectedKey in expectedQueryItems)
            {
                if (!actualQueryItems.AllKeys.Contains(expectedKey))
                {
                    result.AddError(expected: normalisedExpectedQuery, actual: normalisedActualQuery);
                    return result;
                }

                var expectedValue = expectedQueryItems[expectedKey];
                var actualValue = actualQueryItems[expectedKey];

                if (expectedValue != actualValue)
                {
                    result.AddError(expected: normalisedExpectedQuery, actual: normalisedActualQuery);
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