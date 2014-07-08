using System.Collections.Generic;

namespace PactNet.Consumer.Mocks
{
    public interface IMockProvider
    {
        IMockProvider Given(string providerState);
        IMockProvider UponReceiving(string description);
        IMockProvider With(PactProviderServiceRequest request);
        IMockProvider WillRespondWith(PactProviderServiceResponse response);
        IEnumerable<PactInteraction> Interactions { get; }
        void Register();
    }
}