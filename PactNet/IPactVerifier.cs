using System;
using System.Net.Http;

namespace PactNet
{
    public interface IPactVerifier
    {
        IProviderStates ProviderStatesFor(string consumerName, Action setUp = null, Action tearDown = null);
        IPactVerifier ServiceProvider(string providerName, HttpClient httpClient);
        IPactVerifier HonoursPactWith(string consumerName);
        IPactVerifier PactUri(string uri);
        void Verify(string providerDescription = null, string providerState = null);
    }
}