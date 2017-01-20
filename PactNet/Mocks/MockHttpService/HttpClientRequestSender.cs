using System;
using System.Net.Http;
using System.Threading;
using PactNet.Extensions;
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
            //Added because of this http://stackoverflow.com/questions/23438416/why-is-httpclient-baseaddress-not-working
            if (_httpClient.BaseAddress != null && _httpClient.BaseAddress.OriginalString.EndsWith("/"))
            {
                request.Path = request.Path.TrimStart('/');
            }

            var httpRequest = _httpRequestMessageMapper.Convert(request);

            var httpResponse = _httpClient.SendAsync(httpRequest, CancellationToken.None).RunSync();
            var response = _providerServiceResponseMapper.Convert(httpResponse);

            Dispose(httpRequest);
            Dispose(httpResponse);

            return response;
        }

        private static void Dispose(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}