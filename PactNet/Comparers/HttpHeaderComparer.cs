using System;
using System.Collections.Generic;

namespace PactNet.Comparers
{
    public class HttpHeaderComparer : IHttpHeaderComparer
    {
        private readonly string _messagePrefix;

        public HttpHeaderComparer(string messagePrefix)
        {
            _messagePrefix = messagePrefix;
        }

        public void Validate(IDictionary<string, string> headers1, IDictionary<string, string> headers2)
        {
            if (headers2 == null)
            {
                throw new PactComparisonFailed("Headers are null");
            }

            foreach (var header in headers1)
            {
                Console.WriteLine("{0} includes header {1} with value {2}", _messagePrefix, header.Key, header.Value);

                string headerValue;

                if (headers2.TryGetValue(header.Key, out headerValue))
                {
                    if (!header.Value.Equals(headerValue))
                    {
                        throw new PactComparisonFailed(header.Value, headerValue);
                    }
                }
                else
                {
                    throw new PactComparisonFailed("Header does not exist");
                }
            }
        }
    }
}