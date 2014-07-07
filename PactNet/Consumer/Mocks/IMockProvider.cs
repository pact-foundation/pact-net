using System.Collections.Generic;

namespace PactNet.Consumer.Mocks
{
    public interface IMockProvider
    {
        IMockProvider Given(string providerState);
        IMockProvider UponReceiving(string description);
        IMockProvider With(PactProviderRequest request);
        IMockProvider WillRespondWith(PactProviderResponse response);
        IEnumerable<PactInteraction> Interactions { get; }
        void Register();
    }
}