using NSubstitute;
using PactNet.Tests.Fakes;
using PactNet.Validators;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using PactNet.Models.Messaging;
using Xunit;

namespace PactNet.Tests
{
    public class PactMessageingVerifierTests
    {
        private IFileSystem mockFileSystem;
        private FakeHttpMessageHandler fakeHttpMessageHandler;
        private IPactValidator<MessagingPactFile> mockValidator;

        public PactMessageingVerifierTests()
        {
            this.mockFileSystem = Substitute.For<IFileSystem>();
            this.mockValidator = Substitute.For<IPactValidator<MessagingPactFile>>();
            this.fakeHttpMessageHandler = new FakeHttpMessageHandler();
        }

        private IPactMessagingVerifier GetSystemUnderTest()
        {
            return new PactMessagingVerifier(
                null,
                this.mockFileSystem,
                new HttpClient(this.fakeHttpMessageHandler), 
                (reporter, verifierConfig, mockMessager) => this.mockValidator);
        }

        [Fact]
        public void HonoursPactWith_ThrowsExceptionWhenSetToNull()
        {
            IPactMessagingVerifier sut = GetSystemUnderTest();

            Assert.Throws<ArgumentException>(() =>
              sut
              .HonoursPactWith(null));

        }

        [Fact]
        public void HonoursPactWith_ThrowsExceptionWhenSetToWhiteSpace()
        {
            IPactMessagingVerifier sut = GetSystemUnderTest();

            Assert.Throws<ArgumentException>(() =>
              sut
              .HonoursPactWith("  "));

        }

        [Fact]
        public void HonoursPactWith_ThrowsExceptionWhenSetTwice()
        {
            IPactMessagingVerifier sut = GetSystemUnderTest();

            sut.HonoursPactWith("consumer");
            Assert.Throws<ArgumentException>(() =>
              sut
              .HonoursPactWith("consumer2"));

        }

        public void HonoursPactWith_WhenCalledWithValidConsumer_SetsConsumer()
        {
            IPactMessagingVerifier sut = GetSystemUnderTest();

            const string honoursPactWithName = "consumer";
            sut.HonoursPactWith(honoursPactWithName);

            Assert.Equal<string>(honoursPactWithName, ((PactMessagingVerifier)sut).ConsumerName);

        }


        [Fact]
        public void PactUri_WhenCalledWithNullUri_ThrowsArgumentException()
        {
            var pactVerifier = new PactMessagingVerifier();

            Assert.Throws<ArgumentException>(() => pactVerifier.PactUri(null));
        }

        [Fact]
        public void PactUri_WhenCalledWithEmptyUri_ThrowsArgumentException()
        {
            var pactVerifier = new PactMessagingVerifier();

            Assert.Throws<ArgumentException>(() => pactVerifier.PactUri(String.Empty));
        }

        [Fact]
        public void PactUri_WhenCalledWithUri_SetsPactFileUri()
        {
            const string pactFileUri = "../../../Consumer.Tests/pacts/my_client-event_message.json";
            var pactVerifier = new PactMessagingVerifier();

            pactVerifier.PactUri(pactFileUri);

            Assert.Equal(pactFileUri, ((PactMessagingVerifier)pactVerifier).PactFileUri);
        }

        [Fact]
        public void IAmProvider_ThrowsExceptionWhenSetToNull()
        {
            IPactMessagingVerifier sut = GetSystemUnderTest();

            Assert.Throws<ArgumentException>(() =>
              sut
              .IAmProvider(null));

        }

        [Fact]
        public void IAmProvider_ThrowsExceptionWhenSetToEmpty()
        {
            IPactMessagingVerifier sut = GetSystemUnderTest();

            Assert.Throws<ArgumentException>(() =>
              sut
              .IAmProvider(String.Empty));

        }

        [Fact]
        public void IAmProvider_ThrowsExceptionWhenSetTwice()
        {
            IPactMessagingVerifier sut = GetSystemUnderTest();

            sut.IAmProvider("provider");
            Assert.Throws<ArgumentException>(() =>
              sut
              .IAmProvider("provider2"));

        }

        public void IAmProvider_WhenCalledWithValidProvider_SetsProvider()
        {
            IPactMessagingVerifier sut = GetSystemUnderTest();

            const string IAmProviderName = "provider";
            sut.IAmProvider(IAmProviderName);

            Assert.Equal<string>(IAmProviderName, ((PactMessagingVerifier)sut).ProviderName);

        }


        [Fact]
        public void Verify_WithFileSystemPactFileUri_CallsFileReadAllText()
        {
            var messageProvider = "Event Message";
            var messageConsumer = "My client";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_message.json";
            var pactFileJson = "{\"provider\":{\"name\":\"Event Message\"},\"consumer\":{\"name\":\"My client\"},\"messages\":[{\"description\":\"Published credit data\",\"providerState\":\"or maybe \'scenario\'? not sure about this\",\"contents\":{\"foo\":\"bar\"},\"matchingRules\":{\"$.body.foo\":{\"match\":\"type\"}}}],\"metaData\":{\"contentType\":\"application/json\"}}";

            IPactMessagingVerifier sut = GetSystemUnderTest();

            this.mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            sut.IAmProvider(messageProvider)
                .HonoursPactWith(messageConsumer)
                .PactUri(pactUri);

            sut.Verify();

            this.mockFileSystem.File.Received(1).ReadAllText(pactUri);
        }

        [Fact]
        public void Verify_CallsValdiatorWhenPactFileValid()
        {
           
            var messageProvider = "Event Message";
            var messageConsumer = "My client";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_message.json";
            var pactFileJson = "{\"provider\":{\"name\":\"Event Message\"},\"consumer\":{\"name\":\"My client\"},\"messages\":[{\"description\":\"Published credit data\",\"providerState\":\"or maybe \'scenario\'? not sure about this\",\"contents\":{\"foo\":\"bar\"},\"matchingRules\":{\"$.body.foo\":{\"match\":\"type\"}}}],\"metaData\":{\"contentType\":\"application/json\"}}";

            IPactMessagingVerifier sut = GetSystemUnderTest();

            this.mockFileSystem.File.ReadAllText(pactUri).Returns(pactFileJson);

            sut.IAmProvider(messageProvider)
                .HonoursPactWith(messageConsumer)
                .PactUri(pactUri);

            sut.Verify();

            this.mockFileSystem.File.Received(1).ReadAllText(pactUri);
            this.mockValidator.Received(1);
        }

        [Fact]
        public void Verify_WithHttpPactFileUri_CallsHttpClientWithJsonGetRequest()
        {
            var messageProvider = "Event Message";
            var messageConsumer = "My client";
            var pactUri = "http://yourpactserver.com/path/to/pact/file";
            var pactFileJson = "{\"provider\":{\"name\":\"Event Message\"},\"consumer\":{\"name\":\"My client\"},\"messages\":[{\"description\":\"Published credit data\",\"providerState\":\"or maybe \'scenario\'? not sure about this\",\"contents\":{\"foo\":\"bar\"},\"matchingRules\":{\"$.body.foo\":{\"match\":\"type\"}}}],\"metaData\":{\"contentType\":\"application/json\"}}";

            IPactMessagingVerifier sut = GetSystemUnderTest();

            this.fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(pactFileJson, Encoding.UTF8, "application/json")
            };

            sut.IAmProvider(messageProvider)
                .HonoursPactWith(messageConsumer);

            sut.PactUri(pactUri);
            sut.Verify();

            Assert.Equal(HttpMethod.Get, this.fakeHttpMessageHandler.RequestsReceived.Single().Method);
            Assert.Equal("application/json", this.fakeHttpMessageHandler.RequestsReceived.Single().Headers.Single(x => x.Key == "Accept").Value.Single());
        }

        [Fact]
        public void Verify_WithHttpsPactFileUri_GetsAllPactsForProviderWhenBasePactUri()
        {
            var messageProvider = "Event Message";
            var messageConsumer = "My client";
            var pactUri = "https://yourpactserver.com";

            IPactMessagingVerifier sut = GetSystemUnderTest();

            this.fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{result:\"links to pact files\"}", Encoding.UTF8, "application/hal+json")
            };

            sut.IAmProvider(messageProvider)
                .HonoursPactWith(messageConsumer);

            sut.PactUri(pactUri);
            sut.Verify();

            Assert.Equal(HttpMethod.Get, this.fakeHttpMessageHandler.RequestsReceived.Single().Method);
            Assert.Equal("application/hal+json", this.fakeHttpMessageHandler.RequestsReceived.Single().Headers.Single(x => x.Key == "Accept").Value.Single());
        }

        [Fact]
        public void Verify_WithHttpsPactFileUri_GetsAllPactsForProviderWhenActualPactUri()
        {
            var messageProvider = "Event Message";
            var messageConsumer = "My client";
            var pactUri = "https://yourpactserver.com/path/to/pact/file";

            IPactMessagingVerifier sut = GetSystemUnderTest();

            this.fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{result:\"actual pact file\"}", Encoding.UTF8, "application/json")
            };

            sut.IAmProvider(messageProvider)
                .HonoursPactWith(messageConsumer);

            sut.PactUri(pactUri);
            sut.Verify();

            Assert.Equal(HttpMethod.Get, this.fakeHttpMessageHandler.RequestsReceived.Single().Method);
            Assert.Equal("application/json", this.fakeHttpMessageHandler.RequestsReceived.Single().Headers.Single(x => x.Key == "Accept").Value.Single());
        }

        [Fact]
        public void Verify_WithHttpsPactFileUriAndBasicAuthUriOptions_CallsHttpClientWithJsonGetRequestAndBasicAuthorizationHeader()
        {
            var messageProvider = "Event Message";
            var messageConsumer = "My client";
            var pactUri = "https://yourpactserver.com/path/to/pact/file";
            var pactFileJson = "{\"provider\":{\"name\":\"Event Message\"},\"consumer\":{\"name\":\"My client\"},\"messages\":[{\"description\":\"Published credit data\",\"providerState\":\"or maybe \'scenario\'? not sure about this\",\"contents\":{\"foo\":\"bar\"},\"matchingRules\":{\"$.body.foo\":{\"match\":\"type\"}}}],\"metaData\":{\"contentType\":\"application/json\"}}";

            var options = new PactUriOptions("someuser", "somepassword");

            IPactMessagingVerifier sut = GetSystemUnderTest();

            this.fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(pactFileJson, Encoding.UTF8, "application/json")
            };

            sut.IAmProvider(messageProvider)
                .HonoursPactWith(messageConsumer);

            sut.PactUri(pactUri, options);
            sut.Verify();

            Assert.Equal(HttpMethod.Get, this.fakeHttpMessageHandler.RequestsReceived.Single().Method);
            Assert.Equal("application/json", this.fakeHttpMessageHandler.RequestsReceived.Single().Headers.Single(x => x.Key == "Accept").Value.Single());
            Assert.Equal(this.fakeHttpMessageHandler.RequestsReceived.Single().Headers.Authorization.Scheme, options.AuthorizationScheme);
            Assert.Equal(this.fakeHttpMessageHandler.RequestsReceived.Single().Headers.Authorization.Parameter, options.AuthorizationValue);
        }

        [Fact]
        public void Verify_WithFileUriAndWhenFileDoesNotExistOnFileSystem_ThrowsInvalidOperationException()
        {
            var messageProvider = "Event Message";
            var messageConsumer = "My client";
            var pactUri = "../../../Consumer.Tests/pacts/my_client-event_message.json";

            IPactMessagingVerifier sut = GetSystemUnderTest();

            this.mockFileSystem.File.ReadAllText(pactUri).Returns(x => { throw new FileNotFoundException(); });

            sut.IAmProvider(messageProvider)
                .HonoursPactWith(messageConsumer)
                .PactUri(pactUri);

            Assert.Throws<InvalidOperationException>(() => sut.Verify());

            this.mockFileSystem.File.Received(1).ReadAllText(pactUri);
        }

        [Fact]
        public void Verify_WithHttpUriAndNonSuccessfulStatusCodeIsReturned_ThrowsInvalidOperationException()
        {
            var messageProvider = "Event Message";
            var messageConsumer = "My client";
            var pactUri = "http://yourpactserver.com/getlatestpactfile";

            IPactMessagingVerifier sut = GetSystemUnderTest();

            this.fakeHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            sut.IAmProvider(messageProvider)
                .HonoursPactWith(messageConsumer);

            sut.PactUri(pactUri);

            Assert.Throws<InvalidOperationException>(() => sut.Verify());

            Assert.Equal(HttpMethod.Get, this.fakeHttpMessageHandler.RequestsReceived.Single().Method);
        }

    }

}
