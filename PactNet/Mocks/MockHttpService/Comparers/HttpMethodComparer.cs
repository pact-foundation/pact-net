using System;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Reporters;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpMethodComparer : IHttpMethodComparer
    {
        private readonly string _messagePrefix;
        private readonly IReporter _reporter;

        public HttpMethodComparer(string messagePrefix, IReporter reporter)
        {
            _messagePrefix = messagePrefix;
            _reporter = reporter;
        }

        public void Compare(HttpVerb expected, HttpVerb actual)
        {
            _reporter.ReportInfo(String.Format("{0} has method set to {1}", _messagePrefix, expected));
            if (!expected.Equals(actual))
            {
                _reporter.ReportError(expected: expected, actual: actual);
                return;
            }
        }
    }
}