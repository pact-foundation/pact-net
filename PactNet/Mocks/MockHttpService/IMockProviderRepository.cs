using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public interface IMockProviderRepository
    {
        IEnumerable<HandledRequest> HandledRequests { get; }
        void AddHandledRequest(HandledRequest handledRequest);
        void ClearHandledRequests();
    }
}