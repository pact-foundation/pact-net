using System;
using System.Linq;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
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

        public void Compare(PactProviderServiceRequest request1, PactProviderServiceRequest request2)
        {
            if (request1 == null)
            {
                throw new ComparisonFailedException("Expected request cannot be null");
            }

            Console.WriteLine("{0} has method set to {1}", MessagePrefix, request1.Method);
            if (!request1.Method.Equals(request2.Method))
            {
                throw new ComparisonFailedException(request1.Method, request2.Method);
            }

            Console.WriteLine("{0} has path set to {1}", MessagePrefix, request1.Path);
            if (!request1.Path.Equals(request2.Path))
            {
                throw new ComparisonFailedException(request1.Path, request2.Path);
            }

            if (request1.Headers != null && request1.Headers.Any())
            {
                if (request2.Headers == null)
                {
                    throw new ComparisonFailedException("Headers are null");
                }

                _httpHeaderComparer.Compare(request1.Headers, request2.Headers);
            }

            if (request1.Body != null)
            {
                _httpBodyComparer.Validate(request2.Body, request1.Body);
            }
        }
    }
}