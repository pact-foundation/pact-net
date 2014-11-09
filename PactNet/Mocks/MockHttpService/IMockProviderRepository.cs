using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public interface IMockProviderRepository
    {
        ICollection<ProviderServiceInteraction> TestScopedInteractions { get; }
        ICollection<ProviderServiceInteraction> Interactions { get; }
        ICollection<HandledRequest> HandledRequests { get; }

        void AddInteraction(ProviderServiceInteraction interaction);
        void AddHandledRequest(HandledRequest handledRequest);
        ProviderServiceInteraction GetMatchingTestScopedInteraction(ProviderServiceRequest providerServiceRequest);
        void ClearHandledRequests();
        void ClearTestScopedInteractions();
    }
}