using System;
using System.Collections.Generic;
using System.Linq;
using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    internal class HttpHeaderComparer : IHttpHeaderComparer
    {
        public ComparisonResult Compare(IDictionary<string, string> expected, IDictionary<string, string> actual)
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

                string actualValue;

                if (actual.TryGetValue(header.Key, out actualValue))
                {
                    var expectedValue = header.Value;

                    var actualValueSplit = actualValue.Split(new[] { ',', ';' });
                    if (actualValueSplit.Length == 1)
                    {
                        if (!header.Value.Equals(actualValue))
                        {
                            headerResult.RecordFailure(new DiffComparisonFailure(expectedValue, actualValue));
                        }
                    }
                    else
                    {
                        var expectedValueSplit = expectedValue.Split(new[] {',', ';'});
                        var expectedValueSplitJoined = string.Join(",", expectedValueSplit.Select(x => x.Trim()));
                        var actualValueSplitJoined = string.Join(",", actualValueSplit.Select(x => x.Trim()));

                        if (!expectedValueSplitJoined.Equals(actualValueSplitJoined))
                        {
                            headerResult.RecordFailure(new DiffComparisonFailure(expectedValue, actualValue));
                        }
                    }
                }
                else
                {
                    headerResult.RecordFailure(new ErrorMessageComparisonFailure(
                        $"Header with key '{header.Key}', does not exist in actual"));
                }

                result.AddChildResult(headerResult);
            }

            return result;
        }

        private static IDictionary<string, string> MakeDictionaryCaseInsensitive(IDictionary<string, string> from)
        {
            return new Dictionary<string, string>(from, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}