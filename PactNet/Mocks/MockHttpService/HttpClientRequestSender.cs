using System.Net.Http;
using System.Threading;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    internal class HttpClientRequestSender : IHttpRequestSender
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpRequestMessageMapper _httpRequestMessageMapper;
        private readonly IProviderServiceResponseMapper _providerServiceResponseMapper;
        
        internal HttpClientRequestSender(
            HttpClient httpClient, 
            IHttpRequestMessageMapper httpRequestMessageMapper, 
            IProviderServiceResponseMapper providerServiceResponseMapper)
        {
            _httpClient = httpClient;
            _httpRequestMessageMapper = httpRequestMessageMapper;
            _providerServiceResponseMapper = providerServiceResponseMapper;
        }

        public HttpClientRequestSender(HttpClient httpClient)
            : this(httpClient, new HttpRequestMessageMapper(), new ProviderServiceResponseMapper())
        {
        }

        public ProviderServiceResponse Send(ProviderServiceRequest request)
        {
            var httpRequest = _httpRequestMessageMapper.Convert(request);

            var httpResponse = _httpClient.SendAsync(httpRequest, CancellationToken.None).Result;

            return _providerServiceResponseMapper.Convert(httpResponse);
        }
    }
}