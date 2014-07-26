using System;
using System.Linq;
using Nancy.Hosting.Self;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService
{
    public class MockProviderServiceTests
    {
        private IMockProviderService GetSubject(int port = 1234)
        {
            return new MockProviderService(port);
        }

        [Fact]
        public void Ctor_WhenCalledWithPort_SetsBaseUri()
        {
            const int port = 999;
            var expectedBaseUri = String.Format("http://localhost:{0}", port);
            var mockService = GetSubject(port);

            Assert.Equal(expectedBaseUri, mockService.BaseUri);
        }

        [Fact]
        public void Given_WithProviderState_SetsProviderState()
        {
            const string providerState = "My provider state";
            var mockService = GetSubject();

            mockService
                .Given(providerState)
                .UponReceiving("My description")
                .With(new PactProviderServiceRequest())
                .WillRespondWith(new PactProviderServiceResponse());

            var interaction = mockService.Interactions.First() as PactServiceInteraction;
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
                .With(new PactProviderServiceRequest())
                .WillRespondWith(new PactProviderServiceResponse());

            var interaction = mockService.Interactions.First() as PactServiceInteraction;
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
            var request = new PactProviderServiceRequest();
            var mockService = GetSubject();

            mockService.UponReceiving("My description")
                .With(request)
                .WillRespondWith(new PactProviderServiceResponse());
            
            var interaction = mockService.Interactions.First() as PactServiceInteraction;
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
            var response = new PactProviderServiceResponse();
            var mockService = GetSubject();

            mockService.UponReceiving("My description")
                .With(new PactProviderServiceRequest())
                .WillRespondWith(response);

            var interaction = mockService.Interactions.First() as PactServiceInteraction;
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
                .With(new PactProviderServiceRequest())
                .WillRespondWith(new PactProviderServiceResponse());

            mockService
                .UponReceiving("My next description")
                .With(new PactProviderServiceRequest())
                .WillRespondWith(new PactProviderServiceResponse());

            Assert.Equal(2, mockService.Interactions.Count());
        }

        [Fact]
        public void WillRespondWith_WithNullDescription_ThrowsInvalidOperationException()
        {
            var mockService = GetSubject();

            mockService
                .With(new PactProviderServiceRequest());

            Assert.Throws<InvalidOperationException>(() => mockService.WillRespondWith(new PactProviderServiceResponse()));
        }

        [Fact]
        public void WillRespondWith_WithNullRequest_ThrowsInvalidOperationException()
        {
            var mockService = GetSubject();

            mockService
                .UponReceiving("My description");

            Assert.Throws<InvalidOperationException>(() => mockService.WillRespondWith(new PactProviderServiceResponse()));
        }

        [Fact]
        public void WillRespondWith_WithValidInteraction_InteractionIsAdded()
        {
            var providerState = "My provider state";
            var description = "My description";
            var request = new PactProviderServiceRequest();
            var response = new PactProviderServiceResponse();
            var mockService = GetSubject();

            mockService
                .Given(providerState)
                .UponReceiving(description)
                .With(request)
                .WillRespondWith(response);

            var interaction = mockService.Interactions.First() as PactServiceInteraction;

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
            var request = new PactProviderServiceRequest();
            var response = new PactProviderServiceResponse();
            var mockService = GetSubject();

            mockService
                .UponReceiving("My previous description")
                .With(new PactProviderServiceRequest())
                .WillRespondWith(new PactProviderServiceResponse());

            mockService
                .Given(providerState)
                .UponReceiving(description)
                .With(request)
                .WillRespondWith(response);

            var interaction = mockService.Interactions.Last() as PactServiceInteraction;

            Assert.Equal(2, mockService.Interactions.Count());
            Assert.Equal(providerState, interaction.ProviderState);
            Assert.Equal(description, interaction.Description);
            Assert.Equal(request, interaction.Request);
            Assert.Equal(response, interaction.Response);
        }

        [Fact]
        public void Stop_WhenCalled_InteractionsIsNull()
        {
            var mockService = GetSubject();

            mockService
                .UponReceiving("My interaction")
                .With(new PactProviderServiceRequest())
                .WillRespondWith(new PactProviderServiceResponse());

            mockService.Stop();

            Assert.Null(mockService.Interactions);
        }
    }
}
