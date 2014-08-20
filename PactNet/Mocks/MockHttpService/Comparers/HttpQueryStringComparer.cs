namespace PactNet.Mocks.MockHttpService.Comparers
{
    using System;
    using System.Text.RegularExpressions;

    using PactNet.Reporters;

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
            if (String.IsNullOrEmpty(expectedQuery))
            {
                return;
            }

            _reporter.ReportInfo(String.Format("{0} has query set to {1}", _messagePrefix, expectedQuery));
            _reporter.ReportInfo(String.Format("Actual query is {0}", actualQuery));

            if (!ConvertUrlEncodingToUpperCase(expectedQuery).Equals(ConvertUrlEncodingToUpperCase(actualQuery)))
            {
                _reporter.ReportError(expected: ConvertUrlEncodingToUpperCase(expectedQuery), actual: ConvertUrlEncodingToUpperCase(actualQuery));
            }
        }

        private string ConvertUrlEncodingToUpperCase(string query)
        {
            return Regex.Replace(query, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());
        }
    }
}