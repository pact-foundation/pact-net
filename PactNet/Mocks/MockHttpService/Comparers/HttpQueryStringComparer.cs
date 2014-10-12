using System;
using System.Linq;
using System.Text.RegularExpressions;
using Nancy.Helpers;
using PactNet.Reporters;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpQueryStringComparer : IHttpQueryStringComparer
    {
        private readonly string _messagePrefix;
        private readonly IReporter _reporter;

        public HttpQueryStringComparer(string messagePrefix, IReporter reporter)
        {
            _messagePrefix = messagePrefix;
            _reporter = reporter;
        }

        public void Compare(string expected, string actual)
        {
            if (String.IsNullOrEmpty(expected) && String.IsNullOrEmpty(actual))
            {
                return;
            }

            var normalisedExpectedQuery = NormaliseUrlEncodingAndTrimTrailingAmpersand(expected);
            var normalisedActualQuery = NormaliseUrlEncodingAndTrimTrailingAmpersand(actual);

            _reporter.ReportInfo(String.Format("{0} has query set to {1}", _messagePrefix, normalisedExpectedQuery ?? "null"));

            if (expected == null)
            {
                _reporter.ReportError(actual: actual);
                return;
            }

            if (actual == null)
            {
                _reporter.ReportError(expected: expected);
                return;
            }

            var expectedQueryItems = HttpUtility.ParseQueryString(normalisedExpectedQuery);
            var actualQueryItems = HttpUtility.ParseQueryString(normalisedActualQuery);

            if (expectedQueryItems.Count != actualQueryItems.Count)
            {
                _reporter.ReportError(expected: normalisedExpectedQuery, actual: normalisedActualQuery);
                return;
            }

            foreach (string expectedKey in expectedQueryItems)
            {
                if (!actualQueryItems.AllKeys.Contains(expectedKey))
                {
                    _reporter.ReportError(expected: normalisedExpectedQuery, actual: normalisedActualQuery);
                    return;
                }

                var expectedValue = expectedQueryItems[expectedKey];
                var actualValue = actualQueryItems[expectedKey];

                if (expectedValue != actualValue)
                {
                    _reporter.ReportError(expected: normalisedExpectedQuery, actual: normalisedActualQuery);
                    return;
                }
            }
        }

        private string NormaliseUrlEncodingAndTrimTrailingAmpersand(string query)
        {
            return query != null ?
                Regex.Replace(query, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper()).TrimEnd('&') :
                query;
        }
    }
}