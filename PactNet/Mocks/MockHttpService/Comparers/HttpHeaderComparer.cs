using System;
using System.Collections.Generic;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpHeaderComparer : IHttpHeaderComparer
    {
        private readonly string _messagePrefix;

        public HttpHeaderComparer(string messagePrefix)
        {
            _messagePrefix = messagePrefix;
        }

        public void Compare(IDictionary<string, string> headers1, IDictionary<string, string> headers2)
        {
            if (headers2 == null)
            {
                throw new ComparisonFailedException("Headers are null");
            }

            foreach (var header in headers1)
            {
                Console.WriteLine("{0} includes header {1} with value {2}", _messagePrefix, header.Key, header.Value);

                string headerValue;

                if (headers2.TryGetValue(header.Key, out headerValue))
                {
                    if (!header.Value.Equals(headerValue))
                    {
                        throw new ComparisonFailedException(header.Value, headerValue);
                    }
                }
                else
                {
                    throw new ComparisonFailedException("Header does not exist");
                }
            }
        }
    }
}