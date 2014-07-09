using System;
using System.Linq;

namespace PactNet.Comparers
{
    public class PactProviderServiceRequestComparer : IPactProviderServiceRequestComparer
    {
        private readonly IHttpHeaderComparer _httpHeaderComparer;
        private readonly IHttpBodyComparer _httpBodyComparer;

        private const string MessagePrefix = "\t- Request";

        public PactProviderServiceRequestComparer()
        {
            _httpHeaderComparer = new HttpHeaderComparer(MessagePrefix);
            _httpBodyComparer = new HttpBodyComparer(MessagePrefix);
        }

        public void Validate(PactProviderServiceRequest request1, PactProviderServiceRequest request2)
        {
            if (request1 == null)
            {
                throw new PactComparisonFailed("Expected request cannot be null");
            }

            Console.WriteLine("{0} has method set to {1}", MessagePrefix, request1.Method);
            if (!request1.Method.Equals(request2.Method))
            {
                throw new PactComparisonFailed(request1.Method, request2.Method);
            }

            Console.WriteLine("{0} has path set to {1}", MessagePrefix, request1.Path);
            if (!request1.Path.Equals(request2.Path))
            {
                throw new PactComparisonFailed(request1.Path, request2.Path);
            }

            if (request1.Headers != null && request1.Headers.Any())
            {
                if (request2.Headers == null)
                {
                    throw new PactComparisonFailed("Headers are null");
                }

                _httpHeaderComparer.Validate(request1.Headers, request2.Headers);
            }

            if (request1.Body != null)
            {
                _httpBodyComparer.Validate(request2.Body, request1.Body);
            }
        }
    }
}