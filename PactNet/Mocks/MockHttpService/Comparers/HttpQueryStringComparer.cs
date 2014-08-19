using System;
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

        public void Compare(string query1, string query2)
        {
            if (String.IsNullOrEmpty(query1))
            {
                return;
            }

            _reporter.ReportInfo(String.Format("{0} has path set to {1}", _messagePrefix, query1));
            
            if (!query1.Equals(query2))
            {
                _reporter.ReportError(expected: query1, actual: query2);
                return;
            }
        }
    }
}