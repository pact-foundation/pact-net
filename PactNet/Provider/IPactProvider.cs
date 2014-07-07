using System;
using System.Collections.Generic;
using System.Net.Http;

namespace PactNet.Provider
{
    public interface IPactProvider
    {
        string ConsumerName { get; }
        string ProviderName { get; }
        IPactProvider ProviderStatesFor(string consumerName, IDictionary<string, Action> providerStates);
        IPactProvider ServiceProvider(string providerName, HttpClient httpClient);
        IPactProvider HonoursPactWith(string consumerName);
        IPactProvider PactUri(string uri);
        void Execute();
    }
}