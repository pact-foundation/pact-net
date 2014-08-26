using System;
using System.Text.RegularExpressions;
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

        public void Compare(string expectedQuery, string actualQuery)
        {
            if (expectedQuery == null)
            {
                return;
            }

            var normalisedExpectedQuery = ConvertUrlEncodingToUpperCase(expectedQuery);
            var normalisedActualQuery = ConvertUrlEncodingToUpperCase(actualQuery);

            _reporter.ReportInfo(String.Format("{0} has query set to {1}", _messagePrefix, normalisedExpectedQuery));

            if (normalisedExpectedQuery != normalisedActualQuery)
            {
                _reporter.ReportError(expected: normalisedExpectedQuery, actual: normalisedActualQuery);
            }
        }

        private string ConvertUrlEncodingToUpperCase(string query)
        {
            return Regex.Replace(query, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
        }
    }
}