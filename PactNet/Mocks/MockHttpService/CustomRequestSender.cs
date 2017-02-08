using System;
using PactNet.Models.ProviderService;

namespace PactNet.Mocks.MockHttpService
{
    internal class CustomRequestSender : IHttpRequestSender
    {
        private readonly Func<ProviderServiceRequest, ProviderServiceResponse> _httpRequestSenderFunc;

        public CustomRequestSender(Func<ProviderServiceRequest, ProviderServiceResponse> httpRequestSenderFunc)
        {
            _httpRequestSenderFunc = httpRequestSenderFunc;
        }

        public ProviderServiceResponse Send(ProviderServiceRequest request)
        {
            return _httpRequestSenderFunc(request);
        }
    }
}