using System;
using System.Net.Http;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet
{
    public interface IPactVerifier
    {
        IProviderStates ProviderStatesFor(string consumerName, Action setUp = null, Action tearDown = null);
        IPactVerifier ServiceProvider(string providerName, HttpClient httpClient);
        IPactVerifier ServiceProvider(string providerName, Func<ProviderServiceRequest, ProviderServiceResponse> httpRequestSender);
        IPactVerifier HonoursPactWith(string consumerName);
        IPactVerifier PactUri(string uri);
        void Verify(string providerDescription = null, string providerState = null);
    }
}