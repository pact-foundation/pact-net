using System;
using System.Collections.Generic;
using System.Linq;

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
                throw new CompareFailedException("Headers are null");
            }

            headers2 = MakeDictionaryCaseInsensitive(headers2);

            foreach (var header in headers1)
            {
                Console.WriteLine("{0} includes header {1} with value {2}", _messagePrefix, header.Key, header.Value);

                string value2;

                if (headers2.TryGetValue(header.Key, out value2))
                {
                    var value1 = header.Value;

                    var value2Split = value2.Split(new[] {',', ';'});
                    if (value2Split.Length == 1)
                    {
                        if (!header.Value.Equals(value2))
                        {
                            throw new CompareFailedException(header.Value, value2);
                        }
                    }
                    else
                    {
                        var value1Split = value1.Split(new[] {',', ';'});
                        var value1SplitJoined = String.Join(",", value1Split.Select(x => x.Trim()));
                        var value2SplitJoined = String.Join(",", value2Split.Select(x => x.Trim()));

                        if (!value1SplitJoined.Equals(value2SplitJoined))
                        {
                            throw new CompareFailedException(header.Value, value2);
                        }
                    }
                    
                }
                else
                {
                    throw new CompareFailedException("Header does not exist");
                }
            }
        }

        private IDictionary<string, string> MakeDictionaryCaseInsensitive(IDictionary<string, string> from)
        {
            return new Dictionary<string, string>(from, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}