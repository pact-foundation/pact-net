using System;
using System.Collections.Generic;
using System.Linq;
using PactNet.Reporters;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpHeaderComparer : IHttpHeaderComparer
    {
        private readonly string _messagePrefix;
        private readonly IReporter _reporter;

        public HttpHeaderComparer(string messagePrefix, IReporter reporter)
        {
            _messagePrefix = messagePrefix;
            _reporter = reporter;
        }

        public void Compare(IDictionary<string, string> headers1, IDictionary<string, string> headers2)
        {
            if (headers2 == null)
            {
                _reporter.ReportError("Headers are null");
                return;
            }

            headers2 = MakeDictionaryCaseInsensitive(headers2);

            foreach (var header in headers1)
            {
                _reporter.ReportInfo(String.Format("{0} includes header {1} with value {2}", _messagePrefix, header.Key, header.Value));

                string value2;

                if (headers2.TryGetValue(header.Key, out value2))
                {
                    var value1 = header.Value;

                    var value2Split = value2.Split(new[] {',', ';'});
                    if (value2Split.Length == 1)
                    {
                        if (!header.Value.Equals(value2))
                        {
                            _reporter.ReportError(expected: header.Value, actual: value2);
                            return;
                        }
                    }
                    else
                    {
                        var value1Split = value1.Split(new[] {',', ';'});
                        var value1SplitJoined = String.Join(",", value1Split.Select(x => x.Trim()));
                        var value2SplitJoined = String.Join(",", value2Split.Select(x => x.Trim()));

                        if (!value1SplitJoined.Equals(value2SplitJoined))
                        {
                            _reporter.ReportError(expected: header.Value, actual: value2);
                            return;
                        }
                    }
                    
                }
                else
                {
                    _reporter.ReportError("Header does not exist");
                    return;
                }
            }
        }

        private IDictionary<string, string> MakeDictionaryCaseInsensitive(IDictionary<string, string> from)
        {
            return new Dictionary<string, string>(from, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}