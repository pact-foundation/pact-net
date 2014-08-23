using NSubstitute;
using Nancy;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Nancy;
using PactNet.Reporters;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Nancy
{
    public class MockProviderAdminRequestHandlerTests
    {
        private IMockProviderRepository _mockProviderRepository;
        private IReporter _mockReporter;
        private IProviderServiceRequestComparer _mockRequestComparer;

        private IMockProviderAdminRequestHandler GetSubject()
        {
            _mockProviderRepository = Substitute.For<IMockProviderRepository>();
            _mockReporter = Substitute.For<IReporter>();
            _mockRequestComparer = Substitute.For<IProviderServiceRequestComparer>();

            return new MockProviderAdminRequestHandler(
                _mockProviderRepository,
                _mockReporter,
                _mockRequestComparer);
        }

        [Fact]
        public void Handle_WhenADeleteRequestToInteractionsIsMade_ClearHandledRequestsIsCalledOnTheMockProviderRepository()
        {
            var context = new NancyContext
            {
                Request = new Request("DELETE", "/interactions", "http")
            };

            var handler = GetSubject();

            handler.Handle(context);

            _mockProviderRepository.Received(1).ClearHandledRequests();
        }

        [Fact]
        public void Handle_WhenADeleteRequestToInteractionsIsMade_ReturnsOkResponse()
        {
            var context = new NancyContext
            {
                Request = new Request("DELETE", "/interactions", "http")
            };

            var handler = GetSubject();

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WhenALowercasedDeleteRequestToInteractionsIsMade_ReturnsOkResponse()
        {
            var context = new NancyContext
            {
                Request = new Request("delete", "/interactions", "http")
            };

            var handler = GetSubject();

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WhenNoMatchingAdminAction_ReturnsNotFoundResponse()
        {
            var context = new NancyContext
            {
                Request = new Request("GET", "/tester/testing", "http")
            };

            var handler = GetSubject();

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
