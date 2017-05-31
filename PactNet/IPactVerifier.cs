using System;

namespace PactNet
{
    public interface IPactVerifier
    {
        IPactVerifier ProviderState(Uri providerStateSetupUri);
        IPactVerifier ServiceProvider(string providerName, Uri baseUri);
        IPactVerifier HonoursPactWith(string consumerName);
        IPactVerifier PactUri(string uri, PactUriOptions options = null);
        void Verify();
    }
}