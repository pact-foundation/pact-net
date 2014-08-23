using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    //TODO: Add test for this
    public class MockProviderRepository : IMockProviderRepository
    {
        private readonly List<HandledRequest> _handledRequests = new List<HandledRequest>();
        public IEnumerable<HandledRequest> HandledRequests { get { return _handledRequests; } }

        public void AddHandledRequest(HandledRequest handledRequest)
        {
            _handledRequests.Add(handledRequest);
        }

        public void ClearHandledRequests()
        {
            _handledRequests.Clear(); 
        }
    }
}