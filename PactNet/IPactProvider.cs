using System;
using System.Collections.Generic;
using System.Net.Http;

namespace PactNet
{
    public interface IPactProvider
    {
        string ConsumerName { get; }
        string ProviderName { get; }
        string PactFileUri { get; }
        HttpClient HttpClient { get; }
        IReadOnlyDictionary<string, Action> ProviderStates { get; }
        IPactProvider ProviderStatesFor(string consumerName, Dictionary<string, Action> providerStates);
        IPactProvider ServiceProvider(string providerName, HttpClient httpClient);
        IPactProvider HonoursPactWith(string consumerName);
        IPactProvider PactUri(string uri);
        void Verify(string providerDescription = null, string providerState = null);
    }
}