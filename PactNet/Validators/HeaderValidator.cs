using System;
using System.Collections.Generic;
using PactNet.Comparers;

namespace PactNet.Validators
{
    public class HeaderValidator : IHeaderValidator
    {
        private readonly string _messagePrefix;

        public HeaderValidator(string messagePrefix)
        {
            _messagePrefix = messagePrefix;
        }

        public void Validate(IDictionary<string, string> expected, IDictionary<string, string> actual)
        {
            if (actual == null)
            {
                throw new PactComparisonFailed("Headers are null");
            }

            foreach (var header in expected)
            {
                Console.WriteLine("{0} includes header {1} with value {2}", _messagePrefix, header.Key, header.Value);

                string headerValue;

                if (actual.TryGetValue(header.Key, out headerValue))
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