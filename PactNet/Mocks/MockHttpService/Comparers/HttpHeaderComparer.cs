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

        public void Compare(IDictionary<string, string> expected, IDictionary<string, string> actual)
        {
            if (actual == null)
            {
                _reporter.ReportError("Headers are null");
                return;
            }

            actual = MakeDictionaryCaseInsensitive(actual);

            foreach (var header in expected)
            {
                _reporter.ReportInfo(String.Format("{0} includes header {1} with value {2}", _messagePrefix, header.Key, header.Value));

                string actualValue;

                if (actual.TryGetValue(header.Key, out actualValue))
                {
                    var expectedValue = header.Value;

                    var actualValueSplit = actualValue.Split(new[] { ',', ';' });
                    if (actualValueSplit.Length == 1)
                    {
                        if (!header.Value.Equals(actualValue))
                        {
                            _reporter.ReportError(expected: expectedValue, actual: actualValue);
                            return;
                        }
                    }
                    else
                    {
                        var expectedValueSplit = expectedValue.Split(new[] {',', ';'});
                        var expectedValueSplitJoined = String.Join(",", expectedValueSplit.Select(x => x.Trim()));
                        var actualValueSplitJoined = String.Join(",", actualValueSplit.Select(x => x.Trim()));

                        if (!expectedValueSplitJoined.Equals(actualValueSplitJoined))
                        {
                            _reporter.ReportError(expected: expectedValue, actual: actualValue);
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