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

        public void Compare(string path1, string path2)
        {
            if (path1 == null)
            {
                return;
            }

            _reporter.ReportInfo(String.Format("{0} has path set to {1}", _messagePrefix, path1));

            if (!path1.Equals(path2))
            {
                _reporter.ReportError(path1, path2);
                return;
            }
        }
    }
}