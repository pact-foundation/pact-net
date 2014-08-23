using System;
using System.Linq;
using Nancy.Hosting.Self;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Configuration;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Nancy;
using PactNet.Tests.Fakes;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService
{
    public class MockProviderServiceTests
    {
        private FakeHttpClient _fakeHttpClient;

        private IMockProviderService GetSubject(int port = 1234)
        {
            _fakeHttpClient = new FakeHttpClient();

            return new MockProviderService(
                (baseUri, mockContextService) => new NancyHost(new MockProviderNancyBootstrapper(mockContextService), NancyConfig.HostConfiguration, baseUri),
                port,
                baseUri => _fakeHttpClient);
        }

        [Fact]
        public void Ctor_WhenCalledWithPort_SetsBaseUri()
        {
            const int port = 999;
            var expectedBaseUri = String.Format("http://localhost:{0}", port);
            var mockService = GetSubject(port);

            Assert.Equal(expectedBaseUri, ((MockProviderService) mockService).BaseUri);
        }

        [Fact]
        public void Given_WithProviderState_SetsProviderState()
        {
            const string providerState = "My provider state";
            var mockService = GetSubject();

            mockService
                .Given(providerState)
                .UponReceiving("My description")
                .With(new ProviderServiceRequest())
                .WillRespondWith(new ProviderServiceResponse());

            var interaction = mockService.Interactions.First() as ProviderServiceInteraction;
            Assert.Equal(providerState, interaction.ProviderState);
        }

        [Fact]
        public void Given_WithNullProviderState_ThrowsArgumentException()
        {
            var mockService = GetSubject();

            Assert.Throws<ArgumentException>(() => mockService.Given(null));
        }

        [Fact]
        public void Given_WithEmptyProviderState_ThrowsArgumentException()
        {
            var mockService = GetSubject();

            Assert.Throws<ArgumentException>(() => mockService.Given(String.Empty));
        }

        [Fact]
        public void UponReceiving_WithDescription_SetsDescription()
        {
            const string description = "My description";
            var mockService = GetSubject();

            mockService.UponReceiving(description)
                .With(new ProviderServiceRequest())
                .WillRespondWith(new ProviderServiceResponse());

            var interaction = mockService.Interactions.First() as ProviderServiceInteraction;
            Assert.Equal(description, interaction.Description);
        }

        [Fact]
        public void UponReceiving_WithNullDescription_ThrowsArgumentException()
        {
            var mockService = GetSubject();

            Assert.Throws<ArgumentException>(() => mockService.UponReceiving(null));
        }

        [Fact]
        public void UponReceiving_WithEmptyDescription_ThrowsArgumentException()
        {
            var mockService = GetSubject();

            Assert.Throws<ArgumentException>(() => mockService.UponReceiving(String.Empty));
        }

        [Fact]
        public void With_WithRequest_SetsRequest()
        {
            var request = new ProviderServiceRequest();
            var mockService = GetSubject();

            mockService.UponReceiving("My description")
                .With(request)
                .WillRespondWith(new ProviderServiceResponse());

            var interaction = mockService.Interactions.First() as ProviderServiceInteraction;
            Assert.Equal(request, interaction.Request);
        }

        [Fact]
        public void With_WithNullRequest_ThrowsArgumentException()
        {
            var mockService = GetSubject();

            Assert.Throws<ArgumentException>(() => mockService.With(null));
        }

        [Fact]
        public void WillRespondWith_WithResponse_SetsResponse()
        {
            var response = new ProviderServiceResponse();
            var mockService = GetSubject();

            mockService.UponReceiving("My description")
                .With(new ProviderServiceRequest())
                .WillRespondWith(response);

            var interaction = mockService.Interactions.First() as ProviderServiceInteraction;
            Assert.Equal(response, interaction.Response);
        }

        [Fact]
        public void WillRespondWith_WithNullResponse_ThrowsArgumentException()
        {
            var mockService = GetSubject();

            Assert.Throws<ArgumentException>(() => mockService.WillRespondWith(null));
        }

        [Fact]
        public void Interactions_WithNoInteractions_ReturnsNull()
        {
            var mockService = GetSubject();

            Assert.Null(mockService.Interactions);
        }

        [Fact]
        public void Interactions_WithTwoInteractions_ReturnsInteractions()
        {
            var mockService = GetSubject();

            mockService
                .UponReceiving("My description")
                .With(new ProviderServiceRequest())
                .WillRespondWith(new ProviderServiceResponse());

            mockService
                .UponReceiving("My next description")
                .With(new ProviderServiceRequest())
                .WillRespondWith(new ProviderServiceResponse());

            Assert.Equal(2, mockService.Interactions.Count());
        }

        [Fact]
        public void WillRespondWith_WithNullDescription_ThrowsInvalidOperationException()
        {
            var mockService = GetSubject();

            mockService
                .With(new ProviderServiceRequest());

            Assert.Throws<InvalidOperationException>(() => mockService.WillRespondWith(new ProviderServiceResponse()));
        }

        [Fact]
        public void WillRespondWith_WithNullRequest_ThrowsInvalidOperationException()
        {
            var mockService = GetSubject();

            mockService
                .UponReceiving("My description");

            Assert.Throws<InvalidOperationException>(() => mockService.WillRespondWith(new ProviderServiceResponse()));
        }

        [Fact]
        public void WillRespondWith_WithValidInteraction_InteractionIsAdded()
        {
            var providerState = "My provider state";
            var description = "My description";
            var request = new ProviderServiceRequest();
            var response = new ProviderServiceResponse();
            var mockService = GetSubject();

            mockService
                .Given(providerState)
                .UponReceiving(description)
                .With(request)
                .WillRespondWith(response);

            var interaction = mockService.Interactions.First() as ProviderServiceInteraction;

            Assert.Equal(1, mockService.Interactions.Count());
            Assert.Equal(providerState, interaction.ProviderState);
            Assert.Equal(description, interaction.Description);
            Assert.Equal(request, interaction.Request);
            Assert.Equal(response, interaction.Response);
        }

        [Fact]
        public void WillRespondWith_WhenExistingInteractionExistAndWeHaveAnotherValidInteraction_InteractionIsAdded()
        {
            var providerState = "My provider state";
            var description = "My description";
            var request = new ProviderServiceRequest();
            var response = new ProviderServiceResponse();
            var mockService = GetSubject();

            mockService
                .UponReceiving("My previous description")
                .With(new ProviderServiceRequest())
                .WillRespondWith(new ProviderServiceResponse());

            mockService
                .Given(providerState)
                .UponReceiving(description)
                .With(request)
                .WillRespondWith(response);

            var interaction = mockService.Interactions.Last() as ProviderServiceInteraction;

            Assert.Equal(2, mockService.Interactions.Count());
            Assert.Equal(providerState, interaction.ProviderState);
            Assert.Equal(description, interaction.Description);
            Assert.Equal(request, interaction.Request);
            Assert.Equal(response, interaction.Response);
        }

        [Fact]
        public void WillRespondWith_WhenADuplicateInteractionIsAdded_ThrowsInvalidOperationException()
        {
            var providerState = "My provider state";
            var description = "My description";
            var request = new ProviderServiceRequest();
            var response = new ProviderServiceResponse();
            var mockService = GetSubject();

            mockService
                .Given(providerState)
                .UponReceiving(description)
                .With(request)
                .WillRespondWith(response);

            mockService
                .Given(providerState)
                .UponReceiving(description)
                .With(request);

            Assert.Throws<InvalidOperationException>(() => mockService.WillRespondWith(response));
        }

        [Fact]
        public void WillRespondWith_WhenADuplicateInteractionIsAddedWithNoProviderState_ThrowsInvalidOperationException()
        {
            var description = "My description";
            var request = new ProviderServiceRequest();
            var response = new ProviderServiceResponse();
            var mockService = GetSubject();

            mockService
                .UponReceiving(description)
                .With(request)
                .WillRespondWith(response);

            mockService
                .UponReceiving(description)
                .With(request);

            Assert.Throws<InvalidOperationException>(() => mockService.WillRespondWith(response));
        }

        [Fact]
        public void WillRespondWith_WhenAddingADuplicateInteractionAfterClearingInteractions_TheDuplicateInteractionIsNotAdded()
        {
            var providerState = "My provider state";
            var description = "My description";
            var request = new ProviderServiceRequest();
            var response = new ProviderServiceResponse();
            var mockService = GetSubject();

            mockService
                .Given(providerState)
                .UponReceiving(description)
                .With(request)
                .WillRespondWith(response);

            var expectedInteractions = mockService.Interactions;

            mockService.ClearInteractions();

            mockService
                .Given(providerState)
                .UponReceiving(description)
                .With(request)
                .WillRespondWith(response);

            var actualIneractions = mockService.Interactions;

            Assert.Equal(1, actualIneractions.Count());
            Assert.Equal(expectedInteractions.First(), actualIneractions.First());
        }

        [Fact]
        public void Stop_WhenCalled_InteractionsIsNull()
        {
            var mockService = GetSubject();

            mockService
                .UponReceiving("My interaction")
                .With(new ProviderServiceRequest())
                .WillRespondWith(new ProviderServiceResponse());

            mockService.Stop();

            Assert.Null(mockService.Interactions);
        }

        [Fact]
        public void VerifyInteractions_InteractionHasNotBeenUsed_ThrowsInvalidOperationException()
        {
            var mockService = GetSubject();
            var request = new ProviderServiceRequest();

            mockService
                .UponReceiving("My interaction")
                .With(request)
                .WillRespondWith(new ProviderServiceResponse());

            Assert.Throws<InvalidOperationException>(() => mockService.VerifyInteractions());
        }

        [Fact]
        public void VerifyInteractions_InteractionHasBeenUsedMultipleTimes_ThrowsInvalidOperationException()
        {
            var mockService = GetSubject();
            var request = new ProviderServiceRequest();

            mockService
                .UponReceiving("My interaction")
                .With(request)
                .WillRespondWith(new ProviderServiceResponse());

            ((ProviderServiceInteraction)mockService.Interactions.First()).IncrementUsage();
            ((ProviderServiceInteraction)mockService.Interactions.First()).IncrementUsage();

            Assert.Throws<InvalidOperationException>(() => mockService.VerifyInteractions());
        }

        [Fact]
        public void VerifyInteractions_WithInteractionsThatHaveBeenUsedMultipleTimesAndNotUsedAtAll_ThrowsInvalidOperationException()
        {
            var mockService = GetSubject();
            var request = new ProviderServiceRequest();

            mockService
                .UponReceiving("My interaction")
                .With(request)
                .WillRespondWith(new ProviderServiceResponse());

            mockService
                .Given("My provider state")
                .UponReceiving("My interaction 2")
                .With(request)
                .WillRespondWith(new ProviderServiceResponse());

            ((ProviderServiceInteraction)mockService.Interactions.First()).IncrementUsage();
            ((ProviderServiceInteraction)mockService.Interactions.First()).IncrementUsage();

            Assert.Throws<InvalidOperationException>(() => mockService.VerifyInteractions());
        }

        [Fact]
        public void VerifyInteractions_InteractionHasBeenUsed_DoesNotThrow()
        {
            var mockService = GetSubject();
            var request = new ProviderServiceRequest();

            mockService
                .UponReceiving("My interaction")
                .With(request)
                .WillRespondWith(new ProviderServiceResponse());

            ((ProviderServiceInteraction) mockService.Interactions.First()).IncrementUsage();

            mockService.VerifyInteractions();
        }

        [Fact]
        public void VerifyInteractions_WithNoInteractions_DoesNotThrow()
        {
            var mockService = GetSubject();

            mockService.VerifyInteractions();
        }

        [Fact]
        public void ClearInteractions_WhenCalledWithNullHost_DoesNotPerformAdminInteractionsDeleteRequest()
        {
            var mockService = GetSubject();

            mockService.ClearInteractions();

            Assert.Equal(0, _fakeHttpClient.SendAsyncCallCount);
        }

        [Fact]
        public void ClearInteractions_WhenCalledInitialisedHost_PerformsAdminInteractionsDeleteRequest()
        {
            var mockService = GetSubject();

            mockService.ClearInteractions();

            Assert.Equal(1, _fakeHttpClient.SendAsyncCallCount);
        }
    }
}