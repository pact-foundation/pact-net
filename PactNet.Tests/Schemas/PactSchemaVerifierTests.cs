using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using NSubstitute;
using PactNet.Models;
using PactNet.Schemas.Interfaces;
using PactNet.Schemas.Models;
using PactNet.Schemas.Verifiers;
using PactNet.Tests.Fakes;
using Xunit;

namespace PactNet.Tests.Schemas
{
    [SuppressMessage("ReSharper", "ConvertToLambdaExpression")]
    public class PactSchemaVerifierTests
    {
        private IFileSystem _mockFileSystem;
        private IProviderDataSchemaValidator _mockProviderDataSchemaValidator;
        private FakeHttpMessageHandler _fakeHttpMessageHandler;

        private IPactSchemaVerifier GetSubject()
        {
            _mockFileSystem = Substitute.For<IFileSystem>();
            _mockProviderDataSchemaValidator = Substitute.For<IProviderDataSchemaValidator>();
            _fakeHttpMessageHandler = new FakeHttpMessageHandler();

            return new PactSchemaVerifier(() => { }, () => { }, _mockFileSystem, (reporter, config) => { return _mockProviderDataSchemaValidator; }, null, new HttpClient(_fakeHttpMessageHandler));
        }

        [Fact]
        public void ProviderState_WhenCalledWithSetUpAndTearDown_SetsProviderStateWithSetUpAndTearDownActions()
        {
            const string providerState = "If life gives you melons, you must be dyslexic";
            Action providerStateSetUpAction = () => { };
            Action providerStateTearDownAction = () => { };
            
            var pactVerifier = (PactSchemaVerifier)GetSubject();

            pactVerifier.ProviderState(providerState, providerStateSetUpAction, providerStateTearDownAction);

            var providerStateItem = pactVerifier.ProviderStates.Find(providerState);

            Assert.Equal(providerStateSetUpAction, providerStateItem.SetUp);
            Assert.Equal(providerStateTearDownAction, providerStateItem.TearDown);
        }

        [Fact]
        public void ProviderState_WhenCalledWithNullProviderState_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.ProviderState(null));
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
        public void HonoursPactWith_WhenCalledWithNullConsumerName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.HonoursPactWith(null));
        }

        [Fact]
        public void HonoursPactWith_WhenCalledWithEmptyConsumerName_ThrowsArgumentException()
        {
            var pactVerifier = GetSubject();

            Assert.Throws<ArgumentException>(() => pactVerifier.HonoursPactWith(string.Empty));
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

            Assert.Equal(consumerName, ((PactSchemaVerifier)pactVerifier).ConsumerName);
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

            Assert.Throws<ArgumentException>(() => pactVerifier.PactUri(string.Empty));
        }

        [Fact]
        public void PactUri_WhenCalledWithUri_SetsPactFileUri()
        {
            const string pactFileUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactVerifier = GetSubject();

            pactVerifier.PactUri(pactFileUri);

            Assert.Equal(pactFileUri, ((PactSchemaVerifier)pactVerifier).PactFileUri);
        }

        [Fact]
        public void Verify_WithFileSystemPactFileUri_CallsFileReadAllText()
        {
            var serviceConsumer = "My client";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"interactions\": [{ \"description\": \"My Description\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description 2\", \"provider_state\": \"My Provider State\" }, { \"description\": \"My Description\", \"provider_state\": \"My Provider State 2\" }], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";

            var pactVerifier = GetSubject();

            _mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            pactVerifier
                .HonoursPactWith(serviceConsumer)
                .PactUri(pactUri);

            pactVerifier.Verify();

            _mockFileSystem.File.Received(1).ReadAllText(pactUri);
        }

        [Fact]
        public void Verify_WithHttpPactFileUri_CallsHttpClientWithJsonGetRequest()
        {
            var pactUri = "http://yourpactserver.com/getlatestpactfile";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";

            var pactVerifier = GetSubject();
            
            _fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(pactFileJson, Encoding.UTF8, "application/json")
            };

            pactVerifier.PactUri(pactUri);

            pactVerifier.Verify();

            Assert.Equal(HttpMethod.Get, _fakeHttpMessageHandler.RequestsRecieved.Single().Method);
            Assert.Equal("application/json", _fakeHttpMessageHandler.RequestsRecieved.Single().Headers.Single(x => x.Key == "Accept").Value.Single());
        }

        [Fact]
        public void Verify_WithHttpsPactFileUriAndBasicAuthUriOptions_CallsHttpClientWithJsonGetRequestAndBasicAuthorizationHeader()
        {
            var pactUri = "https://yourpactserver.com/getlatestpactfile";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";
            var options = new PactUriOptions("someuser", "somepassword");

            var pactVerifier = GetSubject();

            _fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(pactFileJson, Encoding.UTF8, "application/json")
            };

            pactVerifier.PactUri(pactUri, options);

            pactVerifier.Verify();

            Assert.Equal(HttpMethod.Get, _fakeHttpMessageHandler.RequestsRecieved.Single().Method);

            Assert.Equal("application/json", _fakeHttpMessageHandler.RequestsRecieved.Single().Headers.Single(x => x.Key == "Accept").Value.Single());
            Assert.Equal(_fakeHttpMessageHandler.RequestsRecieved.Single().Headers.Authorization.Scheme, options.AuthorizationScheme);
            Assert.Equal(_fakeHttpMessageHandler.RequestsRecieved.Single().Headers.Authorization.Parameter, options.AuthorizationValue);
        }

        [Fact]
        public void Verify_WithFileUriAndWhenFileDoesNotExistOnFileSystem_ThrowsInvalidOperationException()
        {
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";

            var pactVerifier = GetSubject();

            _mockFileSystem.File.ReadAllText(pactUri).Returns(x => { throw new FileNotFoundException(); });

            pactVerifier.PactUri(pactUri);

            Assert.Throws<InvalidOperationException>(() => pactVerifier.Verify());

            _mockFileSystem.File.Received(1).ReadAllText(pactUri);
        }

        [Fact]
        public void Verify_WithHttpUriAndNonSuccessfulStatusCodeIsReturned_ThrowsInvalidOperationException()
        {
            var pactUri = "http://yourpactserver.com/getlatestpactfile";

            var pactVerifier = GetSubject();

            _fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            pactVerifier.PactUri(pactUri);

            Assert.Throws<InvalidOperationException>(() => pactVerifier.Verify());

            Assert.Equal(HttpMethod.Get, _fakeHttpMessageHandler.RequestsRecieved.Single().Method);
        }

        [Fact]
        public void Verify_WhenPactFileWithNoSchemaExistOnFileSystem_CallsPactProviderValidator()
        {
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";

            var pactVerifier = GetSubject();

            _mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            pactVerifier.PactUri(pactUri);

            pactVerifier.Verify();

            _mockFileSystem.File.Received(1).ReadAllText(pactUri);

            _mockProviderDataSchemaValidator.Received(1).Validate(Arg.Any<ProviderSchemaPactFile>(), Arg.Any<ProviderStates>(), null);
        }

        [Fact]
        public void Verify_WithNoProviderDescriptionOrProviderStateSupplied_CallsValidatorWithAll3Schemas()
        {
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_api.json";
            var pactFileJson = "{ \"provider\": { \"name\": \"Event API\" }, \"consumer\": { \"name\": \"My client\" }, \"schemas\": [{\"schema\":{\"type\":\"object\"}}, {\"schema\":{\"type\":\"object\"}}, {\"schema\":{\"type\":\"object\"}}], \"metadata\": { \"pactSpecificationVersion\": \"1.0.0\" } }";

            var pactVerifier = GetSubject();

            _mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            pactVerifier
                .ProviderState("My Provider State")
                .ProviderState("My Provider State 2");

            pactVerifier.PactUri(pactUri);

            pactVerifier.Verify();

            _mockProviderDataSchemaValidator.Received(1).Validate(Arg.Is<ProviderSchemaPactFile>(x => x.Schemas.Count() == 3), Arg.Any<ProviderStates>(), null);
        }
    }
}
