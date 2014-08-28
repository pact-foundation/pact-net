using System;
using PactNet.Reporters;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpPathComparer : IHttpPathComparer
    {
        private readonly string _messagePrefix;
        private readonly IReporter _reporter;

        public HttpPathComparer(string messagePrefix, IReporter reporter)
        {
            _messagePrefix = messagePrefix;
            _reporter = reporter;
        }

        public void Compare(string expected, string actual)
        {
            if (expected == null)
            {
                return;
            }

            _reporter.ReportInfo(String.Format("{0} has path set to {1}", _messagePrefix, expected));

            if (!expected.Equals(actual))
            {
                _reporter.ReportError(expected: expected, actual: actual);
                return;
            }
        }
    }
}