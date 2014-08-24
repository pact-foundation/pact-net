using System.Linq;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService
{
    public class MockProviderRepositoryTests
    {
        private IMockProviderRepository GetSubject()
        {
            return new MockProviderRepository();
        }

        [Fact]
        public void AddHandledRequest_WithHandledRequest_AddHandledRequest()
        {
            var handledRequest = new HandledRequest(new ProviderServiceRequest(), new ProviderServiceInteraction());
                                     
            var repo = GetSubject();

            repo.AddHandledRequest(handledRequest);

            Assert.Equal(handledRequest, repo.HandledRequests.First());
        }

        [Fact]
        public void ClearHandledRequests_WithNoHandledRequest_HandledRequestIsEmpty()
        {
            var repo = GetSubject();

            repo.ClearHandledRequests();

            Assert.Empty(repo.HandledRequests);
        }

        [Fact]
        public void ClearHandledRequests_WithHandledRequests_HandledRequestIsEmpty()
        {
            var handledRequest1 = new HandledRequest(new ProviderServiceRequest(), new ProviderServiceInteraction());
            var handledRequest2 = new HandledRequest(new ProviderServiceRequest(), new ProviderServiceInteraction());

            var repo = GetSubject();

            repo.AddHandledRequest(handledRequest1);
            repo.AddHandledRequest(handledRequest2);

            repo.ClearHandledRequests();

            Assert.Empty(repo.HandledRequests);
        }
    }
}
