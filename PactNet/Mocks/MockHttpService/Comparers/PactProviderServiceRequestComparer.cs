using System.Linq;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class PactProviderServiceRequestComparer : IPactProviderServiceRequestComparer
    {
        private readonly IHttpMethodComparer _httpMethodComparer;
        private readonly IHttpPathComparer _httpPathComparer;
        private readonly IHttpQueryStringComparer _httpQueryStringComparer;
        private readonly IHttpHeaderComparer _httpHeaderComparer;
        private readonly IHttpBodyComparer _httpBodyComparer;

        private const string MessagePrefix = "\t- Request";

        public PactProviderServiceRequestComparer()
        {
            _httpMethodComparer = new HttpMethodComparer(MessagePrefix);
            _httpPathComparer = new HttpPathComparer(MessagePrefix);
            _httpQueryStringComparer = new HttpQueryStringComparer(MessagePrefix);
            _httpHeaderComparer = new HttpHeaderComparer(MessagePrefix);
            _httpBodyComparer = new HttpBodyComparer(MessagePrefix);
        }

        public void Compare(PactProviderServiceRequest request1, PactProviderServiceRequest request2)
        {
            if (request1 == null)
            {
                throw new CompareFailedException("Expected request cannot be null");
            }

            _httpMethodComparer.Compare(request1.Method, request2.Method);

            _httpPathComparer.Compare(request1.Path, request2.Path);

            _httpQueryStringComparer.Compare(request1.Query, request2.Query);

            if (request1.Headers != null && request1.Headers.Any())
            {
                if (request2.Headers == null)
                {
                    throw new CompareFailedException("Headers are null");
                }

                _httpHeaderComparer.Compare(request1.Headers, request2.Headers);
            }

            if (request1.Body != null)
            {
                _httpBodyComparer.Validate(request2.Body, request1.Body, true);
            }
        }
    }
}