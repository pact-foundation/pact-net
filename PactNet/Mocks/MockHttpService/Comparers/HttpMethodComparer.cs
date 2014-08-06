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

        public void Compare(HttpVerb method1, HttpVerb method2)
        {
            _reporter.ReportInfo(String.Format("{0} has method set to {1}", _messagePrefix, method1));
            if (!method1.Equals(method2))
            {
                _reporter.ReportError(expected: method1, actual: method2);
                return;
            }
        }
    }
}