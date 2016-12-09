using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using NSubstitute;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Validators;
using PactNet.Models;
using PactNet.Tests.Fakes;
using Xunit;
using Xunit.Sdk;

namespace PactNet.Tests
{
    public class PactVerifierTests
    {
        private Tuple<bool, IHttpRequestSender> _providerServiceValidatorFactoryCallInfo;

        private IFileSystem _mockFileSystem;
        private IProviderServiceValidator _mockProviderServiceValidator;
        private FakeHttpMessageHandler _fakeHttpMessageHandler;

        private IPactVerifier GetSubject()
        {
            _providerServiceValidatorFactoryCallInfo = null;
            _mockFileSystem = Substitute.For<IFileSystem>();
            _mockProviderServiceValidator = Substitute.For<IProviderServiceValidator>();
            _fakeHttpMessageHandler = new FakeHttpMessageHandler();

            return new PactVerifier(() => {}, () => {}, _mockFileSystem, (httpRequestSender, reporter, config) =>
            {
                _providerServiceValidatorFactoryCallInfo = new Tuple<bool, IHttpRequestSender>(true, httpRequestSender);
                
                return _mockProviderServiceValidator;
            }, new HttpClient(_fakeHttpMessageHandler), null);
        }

        [Fact]
        public void ProviderState_WhenCalledWithSetUpAndTearDown_SetsProviderStateWithSetUpAndTearDownActions()
        {
            const string providerState = "There is an event with id 1234 in the database";
            Action providerStateSetUpAction = () => { };
            Action providerStateTearDownAction = () => { };

            var pactVerifier = (PactVerifier)GetSubject();

            pactVerifier
                .ProviderState(providerState, providerStateSetUpAction, providerStateTearDownAction);

            var providerStateItem = pactVerifier.ProviderStates.Find(providerState);

            Assert.Equal(providerStateSetUpAction, providerStateItem.SetUp);
            Assert.Equal(providerStateTearDownAction, providerStateItem.TearDown);
        }

        [Fact]
        public void ProviderState_WhenCalledWithNullProviderState_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => 
                pactVerifier
                .ProviderState(null));
        }

        [Fact]
        public void ProviderState_WhenCalledWithEmptyProviderState_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() =>
                pactVerifier
                .ProviderState(String.Empty));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithNullProviderName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.ServiceProvider(null, new HttpClient()));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithEmptyProviderName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.ServiceProvider(String.Empty, new HttpClient()));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithAnAlreadySetProviderName_ThrowsArgumentException()
        {
            const string providerName = "My API";
            var pactVerifier = GetSubject();

            pactVerifier.ServiceProvider(providerName, new HttpClient());

            Assert.Throws<ArgumentException>(() => pactVerifier.ServiceProvider(providerName, new HttpClient()));
        }

        [Fact]
        public void ServiceProviderOverload_WhenCalledWithAnAlreadySetProviderName_ThrowsArgumentException()
        {
            const string providerName = "My API";
            var pactVerifier = GetSubject();

            pactVerifier.ServiceProvider(providerName, request => null);

            Assert.Throws<ArgumentException>(() => pactVerifier.ServiceProvider(providerName, request => null));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithNullHttpClient_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.ServiceProvider("Event API", httpClient: null));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithProviderName_SetsProviderName()
        {
            const string providerName = "Event API";
            var pactVerifier = GetSubject();

            pactVerifier.ServiceProvider(providerName, new HttpClient());

            Assert.Equal(providerName, ((PactVerifier)pactVerifier).ProviderName);
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithHttpClient_HttpClientRequestSenderIsPassedIntoProviderServiceValidatorFactoryWhenVerifyIsCalled()
        {
            var httpClient = new HttpClient();
            var pactVerifier = GetSubject();

            pactVerifier.ServiceProvider("Event API", httpClient);

            pactVerifier
                .HonoursPactWith("My client")
                .PactUri("../../../Consumer.Tests/pacts/my_client-event_api.json")
                .Verify();

            Assert.True(_providerServiceValidatorFactoryCallInfo.Item1, "_providerServiceValidatorFactory was called");
            Assert.IsType(typeof(HttpClientRequestSender), _providerServiceValidatorFactoryCallInfo.Item2); //was called with type
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithNullProviderNameAndCustomRequestSender_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.ServiceProvider(null, request => new ProviderServiceResponse()));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithNullRequestSender_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.ServiceProvider("Event API", (Func<ProviderServiceRequest, ProviderServiceResponse>) null));
        }

        [Fact]
        public void ServiceProvider_WhenCalledWithCustomRequestSender_CustomRequestSenderIsPassedIntoProviderServiceValidatorFactoryWhenVerifyIsCalled()
        {
            var pactVerifier = GetSubject();

            pactVerifier.ServiceProvider("Event API", request => new ProviderServiceResponse());

            pactVerifier
                .HonoursPactWith("My client")
                .PactUri("../../../Consumer.Tests/pacts/my_client-event_api.json")
                .Verify();

            Assert.True(_providerServiceValidatorFactoryCallInfo.Item1, "_providerServiceValidatorFactory was called");
            Assert.IsType(typeof(CustomRequestSender), _providerServiceValidatorFactoryCallInfo.Item2); //was called with type
        }

        [Fact]
        public void HonoursPactWith_WhenCalledWithNullConsumerName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.HonoursPactWith(null));
        }

        [Fact]
        public void HonoursPactWith_WhenCalledWithEmptyConsumerName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.HonoursPactWith(String.Empty));
        }

        [Fact]
        public void HonoursPactWith_WhenCalledWithAnAlreadySetConsumerName_ThrowsArgumentException()
        {
            const string consumerName = "My Consumer";
            var pactVerifier = GetSubject();

            pactVerifier.HonoursPactWith(consumerName);

            Assert.Throws<ArgumentException>(() => pactVerifier.HonoursPactWith(consumerName));
        }

        [Fact]
        public void HonoursPactWith_WhenCalledWithConsumerName_SetsConsumerName()
        {
            const string consumerName = "My Client";
            var pactVerifier = GetSubject();

            pactVerifier.HonoursPactWith(consumerName);

            Assert.Equal(consumerName, ((PactVerifier)pactVerifier).ConsumerName);
        }

        [Fact]
        public void PactUri_WhenCalledWithNullUri_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.PactUri(null));
        }

        [Fact]
        public void PactUri_WhenCalledWithEmptyUri_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.PactUri(String.Empty));
        }

        [Fact]
        public void PactUri_WhenCalledWithUri_SetsPactFileUri()
        {
            const string pactFileUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactVerifier = GetSubject();

            pactVerifier.PactUri(pactFileUri);

            Assert.Equal(pactFileUri, ((PactVerifier)pactVerifier).PactFileUri);
        }

        [Fact]
        public void Verify_WhenHttpRequestSenderIsNull_ThrowsInvalidOperationException()
        {
            var pactVerifier = GetSubject();
            pactVerifier.PactUri("../../../Consumer.Tests/pacts/my_client-event_api.json");

            Assert.Throws<InvalidOperationException>(() => pactVerifier.Verify());
        }

        [Fact]
        public void Verify_WhenPactFileUriIsNull_ThrowsInvalidOperationException()
        {
            var pactVerifier = GetSubject();
            pactVerifier.ServiceProvider("Event API", new HttpClient());

            Assert.Throws<InvalidOperationException>(() => pactVerifier.Verify());
        }

        [Fact]
        public void Verify_WithFileSystemPactFileUri_CallsFileReadAllText()
        {
            var serviceProvider = "Event API";
            var serviceConsumer = "My client";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";

            var pactVerifier = GetSubject();

            _mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            pactVerifier
                .ServiceProvider(serviceProvider, new HttpClient())
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri);

            pactVerifier.Verify();

            _mockFileSystem.File.Received(1).ReadAllText(pactUri);
        }

        [Fact]
        public void Verify_WithHttpPactFileUri_CallsHttpClientWithJsonGetRequest()
        {
            var serviceProvider = "Event API";
            var serviceConsumer = "My client";
            var pactUri = "http://yourpactserver.com/getlatestpactfile";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";

            var pactVerifier = GetSubject();

            _fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(pactFileJson, Encoding.UTF8, "application/json")
            };

            pactVerifier
                .ServiceProvider(serviceProvider, new HttpClient())
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri);

            pactVerifier.Verify();

            Assert.Equal(HttpMethod.Get, _fakeHttpMessageHandler.RequestsReceived.Single().Method);
            Assert.Equal("application/json", _fakeHttpMessageHandler.RequestsReceived.Single().Headers.Single(x => x.Key == "Accept").Value.Single());
        }

        [Fact]
        public void Verify_WithHttpsPactFileUri_CallsHttpClientWithJsonGetRequest()
        {
            var serviceProvider = "Event API";
            var serviceConsumer = "My client";
            var pactUri = "https://yourpactserver.com/getlatestpactfile";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";

            var pactVerifier = GetSubject();

            _fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(pactFileJson, Encoding.UTF8, "application/json")
            };

            pactVerifier
                .ServiceProvider(serviceProvider, new HttpClient())
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri);

            pactVerifier.Verify();

            Assert.Equal(HttpMethod.Get, _fakeHttpMessageHandler.RequestsReceived.Single().Method);
            Assert.Equal("application/json", _fakeHttpMessageHandler.RequestsReceived.Single().Headers.Single(x => x.Key == "Accept").Value.Single());
        }

        [Fact]
        public void Verify_WithHttpsPactFileUriAndBasicAuthUriOptions_CallsHttpClientWithJsonGetRequestAndBasicAuthorizationHeader()
        {
            var serviceProvider = "Event API";
            var serviceConsumer = "My client";
            var pactUri = "https://yourpactserver.com/getlatestpactfile";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var options = new PactUriOptions("someuser", "somepassword");

            var pactVerifier = GetSubject();

            _fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(pactFileJson, Encoding.UTF8, "application/json")
            };

            pactVerifier
                .ServiceProvider(serviceProvider, new HttpClient())
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri, options);

            pactVerifier.Verify();

            Assert.Equal(HttpMethod.Get, _fakeHttpMessageHandler.RequestsReceived.Single().Method);
            Assert.Equal("application/json", _fakeHttpMessageHandler.RequestsReceived.Single().Headers.Single(x => x.Key == "Accept").Value.Single());
            Assert.Equal(_fakeHttpMessageHandler.RequestsReceived.Single().Headers.Authorization.Scheme, options.AuthorizationScheme);
            Assert.Equal(_fakeHttpMessageHandler.RequestsReceived.Single().Headers.Authorization.Parameter, options.AuthorizationValue);
        }

        [Fact]
        public void Verify_WithFileUriAndWhenFileDoesNotExistOnFileSystem_ThrowsInvalidOperationException()
        {
            var serviceProvider = "Event API";
            var serviceConsumer = "My client";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";

            var pactVerifier = GetSubject();

            _mockFileSystem.File.ReadAllText(pactUri).Returns(x => { throw new FileNotFoundException(); });

            pactVerifier
                .ServiceProvider(serviceProvider, new HttpClient())
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri);

            Assert.Throws<InvalidOperationException>(() => pactVerifier.Verify());

            _mockFileSystem.File.Received(1).ReadAllText(pactUri);
        }

        [Fact]
        public void Verify_WithHttpUriAndNonSuccessfulStatusCodeIsReturned_ThrowsInvalidOperationException()
        {
            var serviceProvider = "Event API";
            var serviceConsumer = "My client";
            var pactUri = "http://yourpactserver.com/getlatestpactfile";

            var pactVerifier = GetSubject();

            _fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            pactVerifier
                .ServiceProvider(serviceProvider, new HttpClient())
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri);

            Assert.Throws<InvalidOperationException>(() => pactVerifier.Verify());

            Assert.Equal(HttpMethod.Get, _fakeHttpMessageHandler.RequestsReceived.Single().Method);
        }

        [Fact]
        public void Verify_WhenPactFileWithNoInteractionsExistOnFileSystem_CallsPactProviderValidator()
        {
            var serviceProvider = "Event API";
            var serviceConsumer = "My client";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"" + serviceProvider + "\" }, \"consumer\": { \"name\": \"" + serviceConsumer + "\" }, \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var pactVerifier = GetSubject();

            _mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            pactVerifier
                .ServiceProvider(serviceProvider, httpClient)
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri);

            pactVerifier.Verify();

            _mockFileSystem.File.Received(1).ReadAllText(pactUri);

            _mockProviderServiceValidator.Received(1).Validate(Arg.Any<ProviderServicePactFile>(), Arg.Any<ProviderStates>());
        }

        [Fact]
        public void Verify_WithNoProviderDescriptionOrProviderStateSupplied_CallsProviderServiceValidatorWithAll3Interactions()
        {
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var pactVerifier = GetSubject();

            _mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            pactVerifier
                .ProviderState("My Provider State")
                .ProviderState("My Provider State 2");

            pactVerifier.ServiceProvider("Event API", httpClient)
                .HonoursPactWith("My client")
                .PactUri(pactUri);

            pactVerifier.Verify();

            _mockProviderServiceValidator.Received(1).Validate(
                Arg.Is<ProviderServicePactFile>(x => x.Interactions.Count() == 3),
                Arg.Any<ProviderStates>());
        }

        [Fact]
        public void Verify_WithDescription_CallsProviderServiceValidatorWith2FilteredInteractions()
        {
            var description = "My Description";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var pactVerifier = GetSubject();

            _mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            pactVerifier
                .ProviderState("My Provider State")
                .ProviderState("My Provider State 2");

            pactVerifier.ServiceProvider("Event API", httpClient)
                .HonoursPactWith("My client")
                .PactUri(pactUri);

            pactVerifier.Verify(description: description);

            _mockProviderServiceValidator.Received(1).Validate(
                Arg.Is<ProviderServicePactFile>(x => x.Interactions.Count() == 2 && x.Interactions.All(i => i.Description.Equals(description))),
                Arg.Any<ProviderStates>());
        }

        [Fact]
        public void Verify_WithProviderState_CallsProviderServiceValidatorWith2FilteredInteractions()
        {
            var providerState = "My Provider State";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var pactVerifier = GetSubject();

            _mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            pactVerifier
                .ProviderState("My Provider State")
                .ProviderState("My Provider State 2");

            pactVerifier.ServiceProvider("Event API", httpClient)
                .HonoursPactWith("My client")
                .PactUri(pactUri);

            pactVerifier.Verify(providerState: providerState);

            _mockProviderServiceValidator.Received(1).Validate(
                Arg.Is<ProviderServicePactFile>(x => x.Interactions.Count() == 2 && x.Interactions.All(i => i.ProviderState.Equals(providerState))), 
                Arg.Any<ProviderStates>());
        }

        [Fact]
        public void Verify_WithDescriptionAndProviderState_CallsProviderServiceValidatorWith1FilteredInteractions()
        {
            var description = "My Description";
            var providerState = "My Provider State";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var pactVerifier = GetSubject();

            _mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            pactVerifier
                .ProviderState("My Provider State")
                .ProviderState("My Provider State 2");

            pactVerifier.ServiceProvider("Event API", httpClient)
                .HonoursPactWith("My client")
                .PactUri(pactUri);

            pactVerifier.Verify(description: description, providerState: providerState);

            _mockProviderServiceValidator.Received(1).Validate(
                Arg.Is<ProviderServicePactFile>(x => x.Interactions.Count() == 1 && x.Interactions.All(i => i.ProviderState.Equals(providerState) && i.Description.Equals(description))), 
                Arg.Any<ProviderStates>());
        }

        [Fact]
        public void Verify_WithDescriptionThatYieldsNoInteractions_ThrowsArgumentException()
        {
            var description = "Description that does not match an interaction";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var httpClient = new HttpClient();

            var pactVerifier = GetSubject();

            _mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            pactVerifier
                .ProviderState("My Provider State")
                .ProviderState("My Provider State 2");

            pactVerifier.ServiceProvider("Event API", httpClient)
                .HonoursPactWith("My client")
                .PactUri(pactUri);

            Assert.Throws<ArgumentException>(() => pactVerifier.Verify(description: description));

            _mockProviderServiceValidator.DidNotReceive().Validate(
                Arg.Any<ProviderServicePactFile>(),
                Arg.Any<ProviderStates>());
        }

        [Fact]
        public void Verify_WithProviderStateSet_CallsProviderServiceValidatorWithProviderState()
        {
            var httpClient = new HttpClient();

            var pactVerifier = GetSubject();

            _mockFileSystem.File.ReadAllText(Arg.Any<string>()).Returns("{}");

            pactVerifier
                .ProviderState("My Provider State")
                .ProviderState("My Provider State 2");

            pactVerifier.ServiceProvider("Event API", httpClient)
                .HonoursPactWith("My client")
                .PactUri("/test.json");

            pactVerifier.Verify();

            _mockProviderServiceValidator.Received(1).Validate(
                Arg.Any<ProviderServicePactFile>(),
                (pactVerifier as PactVerifier).ProviderStates);
        }
    }
}
