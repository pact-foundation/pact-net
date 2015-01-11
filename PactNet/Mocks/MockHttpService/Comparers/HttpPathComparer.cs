using System;
using PactNet.Comparers;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpPathComparer : IHttpPathComparer
    {
        private readonly string _messagePrefix;

        public HttpPathComparer(string messagePrefix)
        {
            _messagePrefix = messagePrefix;
        }

        public ComparisonResult Compare(string expected, string actual)
        {
            var result = new ComparisonResult();

            if (expected == null)
            {
                return result;
            }

            result.AddInfo(String.Format("{0} has path set to {1}", _messagePrefix, expected));

            if (!expected.Equals(actual))
            {
                result.AddError(expected: expected, actual: actual);
                return result;
            }

            return result;
        }
    }
}