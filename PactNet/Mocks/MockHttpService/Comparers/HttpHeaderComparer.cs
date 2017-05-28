using System;
using System.Collections.Generic;
using System.Linq;
using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpHeaderComparer : IHttpHeaderComparer
    {
        public ComparisonResult Compare(IDictionary<string, object> expected, IDictionary<string, object> actual)
        {
            var result = new ComparisonResult("includes headers");

            if (actual == null)
            {
                result.RecordFailure(new ErrorMessageComparisonFailure("Actual Headers are null"));
                return result;
            }

            actual = MakeDictionaryCaseInsensitive(actual);

            foreach (var header in expected)
            {
                var headerResult = new ComparisonResult("'{0}' with value {1}", header.Key, header.Value);

                object actualValue;

                if (actual.TryGetValue(header.Key, out actualValue))
                {
                    var expectedValue = header.Value;

                    var actualValueSplit = actualValue.ToString().Split(new[] { ',', ';' });
                    if (actualValueSplit.Length == 1)
                    {
                        if (!header.Value.Equals(actualValue))
                        {
                            headerResult.RecordFailure(new DiffComparisonFailure(expectedValue, actualValue));
                        }
                    }
                    else
                    {
                        var expectedValueSplit = expectedValue.ToString().Split(new[] {',', ';'});
                        var expectedValueSplitJoined = String.Join(",", expectedValueSplit.Select(x => x.Trim()));
                        var actualValueSplitJoined = String.Join(",", actualValueSplit.Select(x => x.Trim()));

                        if (!expectedValueSplitJoined.Equals(actualValueSplitJoined))
                        {
                            headerResult.RecordFailure(new DiffComparisonFailure(expectedValue, actualValue));
                        }
                    }
                }
                else
                {
                    headerResult.RecordFailure(new ErrorMessageComparisonFailure(String.Format("Header with key '{0}', does not exist in actual", header.Key)));
                }

                result.AddChildResult(headerResult);
            }

            return result;
        }

        private IDictionary<string, object> MakeDictionaryCaseInsensitive(IDictionary<string, object> from)
        {
            return new Dictionary<string, object>(from, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}