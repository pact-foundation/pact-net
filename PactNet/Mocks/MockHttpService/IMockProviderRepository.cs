using System.Collections.Generic;
using PactNet.Models.ProviderService;

namespace PactNet.Mocks.MockHttpService
{
    internal interface IMockProviderRepository
    {
        string TestContext { get; set; }
        ICollection<ProviderServiceInteraction> TestScopedInteractions { get; }
        ICollection<ProviderServiceInteraction> Interactions { get; }
        ICollection<HandledRequest> HandledRequests { get; }

        void AddInteraction(ProviderServiceInteraction interaction);
        void AddHandledRequest(HandledRequest handledRequest);
        ProviderServiceInteraction GetMatchingTestScopedInteraction(ProviderServiceRequest providerServiceRequest);
        void ClearTestScopedState();
    }
}