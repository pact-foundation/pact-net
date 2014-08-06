using System.Linq;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Reporters;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class ProviderServiceRequestComparer : IProviderServiceRequestComparer
    {
        private readonly IHttpMethodComparer _httpMethodComparer;
        private readonly IHttpPathComparer _httpPathComparer;
        private readonly IHttpQueryStringComparer _httpQueryStringComparer;
        private readonly IHttpHeaderComparer _httpHeaderComparer;
        private readonly IHttpBodyComparer _httpBodyComparer;
        private readonly IReporter _reporter;

        private const string MessagePrefix = "\t- Request";

        public ProviderServiceRequestComparer(IReporter reporter)
        {
            _reporter = reporter;
            _httpMethodComparer = new HttpMethodComparer(MessagePrefix, _reporter);
            _httpPathComparer = new HttpPathComparer(MessagePrefix, _reporter);
            _httpQueryStringComparer = new HttpQueryStringComparer(MessagePrefix, _reporter);
            _httpHeaderComparer = new HttpHeaderComparer(MessagePrefix, _reporter);
            _httpBodyComparer = new HttpBodyComparer(MessagePrefix, _reporter);
        }

        public void Compare(ProviderServiceRequest request1, ProviderServiceRequest request2)
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