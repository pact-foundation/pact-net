using System;
using System.Collections.Generic;
using System.Linq;
using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpHeaderComparer : IHttpHeaderComparer
    {
        private readonly string _messagePrefix;

        public HttpHeaderComparer(string messagePrefix)
        {
            _messagePrefix = messagePrefix;
        }

        public ComparisonResult Compare(IDictionary<string, string> expected, IDictionary<string, string> actual)
        {
            var result = new ComparisonResult();

            if (actual == null)
            {
                result.AddError("Headers are null");
                return result;
            }

            actual = MakeDictionaryCaseInsensitive(actual);

            foreach (var header in expected)
            {
                result.AddInfo(String.Format("{0} includes header {1} with value {2}", _messagePrefix, header.Key, header.Value));

                string actualValue;

                if (actual.TryGetValue(header.Key, out actualValue))
                {
                    var expectedValue = header.Value;

                    var actualValueSplit = actualValue.Split(new[] { ',', ';' });
                    if (actualValueSplit.Length == 1)
                    {
                        if (!header.Value.Equals(actualValue))
                        {
                            result.AddError(expected: expectedValue, actual: actualValue);
                            return result;
                        }
                    }
                    else
                    {
                        var expectedValueSplit = expectedValue.Split(new[] {',', ';'});
                        var expectedValueSplitJoined = String.Join(",", expectedValueSplit.Select(x => x.Trim()));
                        var actualValueSplitJoined = String.Join(",", actualValueSplit.Select(x => x.Trim()));

                        if (!expectedValueSplitJoined.Equals(actualValueSplitJoined))
                        {
                            result.AddError(expected: expectedValue, actual: actualValue);
                            return result;
                        }
                    }
                    
                }
                else
                {
                    result.AddError("Header does not exist");
                    return result;
                }
            }

            return result;
        }

        private IDictionary<string, string> MakeDictionaryCaseInsensitive(IDictionary<string, string> from)
        {
            return new Dictionary<string, string>(from, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}