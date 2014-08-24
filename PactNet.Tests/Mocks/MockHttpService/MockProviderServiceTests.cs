using System;
using System.Linq;
using NSubstitute;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Tests.Fakes;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService
{
    public class MockProviderServiceTests
    {
        private IHttpHost _mockHttpHost;
        private FakeHttpClient _fakeHttpClient;
        private int _mockHttpHostFactoryCallCount;

        private IMockProviderService GetSubject(int port = 1234)
        {
            _mockHttpHost = Substitute.For<IHttpHost>();
            _fakeHttpClient = new FakeHttpClient();
            _mockHttpHostFactoryCallCount = 0;

            return new MockProviderService(
                (baseUri, mockContextService) =>
                {
                    _mockHttpHostFactoryCallCount++;
                    return _mockHttpHost;
                },
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
        public void VerifyInteractions_WhenHostIsNull_ThrowsInvalidOperationException()
        {
            var mockService = GetSubject();

            mockService.Stop();

            Assert.Throws<InvalidOperationException>(() => mockService.VerifyInteractions());
        }

        [Fact]
        public void VerifyInteractions_WhenHostIsNull_DoesNotPerformAdminInteractionsVerificationGetRequest()
        {
            var mockService = GetSubject();

            mockService.Stop();

            try
            {
                mockService.VerifyInteractions();
            }
            catch (Exception)
            {
            }

            Assert.Equal(0, _fakeHttpClient.SendAsyncCallCount);
        }

        [Fact]
        public void VerifyInteractions_WhenHostIsNotNull_PerformsAdminInteractionsVerificationGetRequest()
        {
            var mockService = GetSubject();

            mockService.Start();

            mockService.VerifyInteractions();

            Assert.Equal(1, _fakeHttpClient.SendAsyncCallCount);
        }

        [Fact]
        public void ClearInteractions_WhenHostIsNull_DoesNotPerformAdminInteractionsDeleteRequest()
        {
            var mockService = GetSubject();

            mockService.Stop();

            mockService.ClearInteractions();

            Assert.Equal(0, _fakeHttpClient.SendAsyncCallCount);
        }

        [Fact]
        public void ClearInteractions_WhenHostIsNotNull_PerformsAdminInteractionsDeleteRequest()
        {
            var mockService = GetSubject();

            mockService.Start();

            mockService.ClearInteractions();

            Assert.Equal(1, _fakeHttpClient.SendAsyncCallCount);
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
        public void Stop_WithNullHost_DoesNotThrow()
        {
            var mockService = GetSubject();

            mockService.Stop();
        }

        [Fact]
        public void Stop_WithNonNullHost_StopIsCalledOnHttpHost()
        {
            var mockService = GetSubject();

            mockService.Start();

            mockService.Stop();

            _mockHttpHost.Received(1).Stop();
        }

        [Fact]
        public void Start_WithNonNullHost_StopIsCalledOnHttpHost()
        {
            var mockService = GetSubject();

            mockService.Start();

            mockService.Start();

            _mockHttpHost.Received(1).Stop();
        }

        [Fact]
        public void Start_WithNullHost_DoesNotThrow()
        {
            var mockService = GetSubject();

            mockService.Start();
        }

        [Fact]
        public void Start_WhenCalled_CallsHostFactory()
        {
            var mockService = GetSubject();

            mockService.Start();

            Assert.Equal(1, _mockHttpHostFactoryCallCount);
        }

        [Fact]
        public void Start_WhenCalled_CallsStartOnHttpHost()
        {
            var mockService = GetSubject();

            mockService.Start();

            _mockHttpHost.Received(1).Start();
        }
    }
}