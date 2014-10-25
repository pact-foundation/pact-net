using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using NSubstitute;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Tests.Fakes;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService
{
    public class MockProviderServiceTests
    {
        private IHttpHost _mockHttpHost;
        private FakeHttpMessageHandler _fakeHttpMessageHandler;
        private int _mockHttpHostFactoryCallCount;

        private IMockProviderService GetSubject(int port = 1234, bool enableSsl = false)
        {
            _mockHttpHost = Substitute.For<IHttpHost>();
            _fakeHttpMessageHandler = new FakeHttpMessageHandler();
            _mockHttpHostFactoryCallCount = 0;

            return new MockProviderService(
                baseUri =>
                {
                    _mockHttpHostFactoryCallCount++;
                    return _mockHttpHost;
                },
                port,
                enableSsl,
                baseUri => new HttpClient(_fakeHttpMessageHandler) { BaseAddress = new Uri(baseUri) },
                new HttpMethodMapper());
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
        public void Ctor_WhenCalledWithEnableSslFalse_SetsBaseUriWithHttpScheme()
        {
            var mockService = GetSubject(enableSsl: false);

            Assert.True(((MockProviderService)mockService).BaseUri.StartsWith("http"), "BaseUri has a http scheme");
        }

        [Fact]
        public void Ctor_WhenCalledWithEnableSslTrue_SetsBaseUriWithHttpsScheme()
        {
            var mockService = GetSubject(enableSsl: true);

            Assert.True(((MockProviderService)mockService).BaseUri.StartsWith("https"), "BaseUri has a https scheme");
        }

        [Fact]
        public void Given_WithProviderState_SetsProviderState()
        {
            const string providerState = "My provider state";
            var mockService = GetSubject();
            mockService.Start();

            mockService
                .Given(providerState)
                .UponReceiving("My description")
                .With(new ProviderServiceRequest())
                .WillRespondWith(new ProviderServiceResponse());

            var interaction = Deserialise<ProviderServiceInteraction>(_fakeHttpMessageHandler.RequestContentRecieved.Single());

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
            mockService.Start();

            mockService.UponReceiving(description)
                .With(new ProviderServiceRequest())
                .WillRespondWith(new ProviderServiceResponse());

            var interaction = Deserialise<ProviderServiceInteraction>(_fakeHttpMessageHandler.RequestContentRecieved.Single());
            
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
            var description = "My description";
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Head,
                Path = "/tester/testing/1"
            };
            var response = new ProviderServiceResponse
            {
                Status = (int)HttpStatusCode.ProxyAuthenticationRequired
            };

            var expectedInteraction = new ProviderServiceInteraction
            {
                Description = description,
                Request = request,
                Response = response
            };
            var expectedInteractionJson = expectedInteraction.AsJsonString();

            var mockService = GetSubject();
            mockService.Start();

            mockService.UponReceiving(description)
                .With(request)
                .WillRespondWith(response);

            var actualInteractionJson = _fakeHttpMessageHandler.RequestContentRecieved.Single();
            
            Assert.Equal(expectedInteractionJson, actualInteractionJson);
        }

        [Fact]
        public void With_WithNullRequest_ThrowsArgumentException()
        {
            var mockService = GetSubject();

            Assert.Throws<ArgumentException>(() => mockService.With(null));
        }

        [Fact]
        public void WillRespondWith_WithNullResponse_ThrowsArgumentException()
        {
            var mockService = GetSubject();

            Assert.Throws<ArgumentException>(() => mockService.WillRespondWith(null));
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
        public void WillRespondWith_WhenHostIsNull_ThrowsInvalidOperationException()
        {
            var providerState = "My provider state";
            var description = "My description";
            var request = new ProviderServiceRequest();
            var response = new ProviderServiceResponse();

            var mockService = GetSubject();

            mockService
                .Given(providerState)
                .UponReceiving(description)
                .With(request);

            mockService.Stop();

            Assert.Throws<InvalidOperationException>(() => mockService.WillRespondWith(response));
            Assert.Equal(0, _fakeHttpMessageHandler.RequestsRecieved.Count());
        }

        [Fact]
        public void WillRespondWith_WithValidInteraction_PerformsAdminInteractionsPostRequestWithInteraction()
        {
            var providerState = "My provider state";
            var description = "My description";
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Head,
                Path = "/tester/testing/1"
            };
            var response = new ProviderServiceResponse
            {
                Status = (int)HttpStatusCode.ProxyAuthenticationRequired
            };

            var expectedInteraction = new ProviderServiceInteraction
            {
                ProviderState = providerState,
                Description = description,
                Request = request,
                Response = response
            };
            var expectedInteractionJson = expectedInteraction.AsJsonString();

            var mockService = GetSubject();
            mockService.Start();

            mockService
                .Given(providerState)
                .UponReceiving(description)
                .With(request)
                .WillRespondWith(response);

            var actualRequest = _fakeHttpMessageHandler.RequestsRecieved.Single();
            var actualInteractionJson = _fakeHttpMessageHandler.RequestContentRecieved.Single();

            Assert.Equal(HttpMethod.Post, actualRequest.Method);
            Assert.Equal("http://localhost:1234/interactions", actualRequest.RequestUri.OriginalString);
            Assert.True(actualRequest.Headers.Contains(Constants.AdministrativeRequestHeaderKey));

            Assert.Equal(expectedInteractionJson, actualInteractionJson);
        }

        [Fact]
        public void WillRespondWith_WhenResponseFromHostIsNotOk_ThrowsPactFailureException()
        {
            var providerState = "My provider state";
            var description = "My description";
            var request = new ProviderServiceRequest();
            var response = new ProviderServiceResponse();

            var mockService = GetSubject();

            _fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            mockService
                .Given(providerState)
                .UponReceiving(description)
                .With(request);

            mockService.Start();

            Assert.Throws<PactFailureException>(() => mockService.WillRespondWith(response));
        }

        [Fact]
        public void VerifyInteractions_WhenHostIsNull_ThrowsInvalidOperationException()
        {
            var mockService = GetSubject();

            mockService.Stop();

            Assert.Throws<InvalidOperationException>(() => mockService.VerifyInteractions());
            Assert.Equal(0, _fakeHttpMessageHandler.RequestsRecieved.Count());
        }

        [Fact]
        public void VerifyInteractions_WhenHostIsNotNull_PerformsAdminInteractionsVerificationGetRequest()
        {
            var mockService = GetSubject();

            mockService.Start();

            mockService.VerifyInteractions();

            Assert.Equal(1, _fakeHttpMessageHandler.RequestsRecieved.Count());
            Assert.Equal(HttpMethod.Get, _fakeHttpMessageHandler.RequestsRecieved.First().Method);
            Assert.Equal("http://localhost:1234/interactions/verification", _fakeHttpMessageHandler.RequestsRecieved.First().RequestUri.ToString());
        }

        [Fact]
        public void VerifyInteractions_WhenResponseFromHostIsNotOk_ThrowsPactFailureException()
        {
            var mockService = GetSubject();

            _fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);

            mockService.Start();

            Assert.Throws<PactFailureException>(() => mockService.VerifyInteractions());
        }

        [Fact]
        public void ClearInteractions_WhenHostIsNull_DoesNotPerformAdminInteractionsDeleteRequest()
        {
            var mockService = GetSubject();
            mockService.Stop();

            mockService.ClearInteractions();

            Assert.Equal(0, _fakeHttpMessageHandler.RequestsRecieved.Count());
        }

        [Fact]
        public void ClearInteractions_WhenHostIsNotNull_PerformsAdminInteractionsDeleteRequest()
        {
            var mockService = GetSubject();

            mockService.Start();

            mockService.ClearInteractions();

            Assert.Equal(1, _fakeHttpMessageHandler.RequestsRecieved.Count());
            Assert.Equal(HttpMethod.Delete, _fakeHttpMessageHandler.RequestsRecieved.First().Method);
            Assert.Equal("http://localhost:1234/interactions", _fakeHttpMessageHandler.RequestsRecieved.First().RequestUri.ToString());
        }

        [Fact]
        public void ClearInteractions_WhenResponseFromHostIsNotOk_ThrowsPactFailureException()
        {
            var mockService = GetSubject();

            _fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            mockService.Start();

            Assert.Throws<PactFailureException>(() => mockService.ClearInteractions());
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

        private T Deserialise<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonConfig.ApiSerializerSettings);
        }
    }
}