using System;
using System.Threading.Tasks;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    internal class CustomRequestSender : IHttpRequestSender
    {
        private readonly Func<ProviderServiceRequest, ProviderServiceResponse> _httpRequestSenderFunc;

        public CustomRequestSender(Func<ProviderServiceRequest, ProviderServiceResponse> httpRequestSenderFunc)
        {
            _httpRequestSenderFunc = httpRequestSenderFunc;
        }

        public Task<ProviderServiceResponse> Send(ProviderServiceRequest request)
        {
            return Task.FromResult(_httpRequestSenderFunc(request));
        }
    }
}