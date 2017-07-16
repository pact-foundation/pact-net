using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using PactNet.Models;
using PactNet.Tests.Fakes;
using Xunit;

namespace PactNet.Tests
{
    public class PactPublisherTests
    {
        private FakeHttpMessageHandler _fakeHttpMessageHandler;

        private PactPublisher GetSubject(string brokerBaseUri, PactUriOptions brokerUriOptions = null)
        {
            _fakeHttpMessageHandler = new FakeHttpMessageHandler();

            return new PactPublisher(brokerBaseUri, brokerUriOptions, _fakeHttpMessageHandler);
        }

        [Fact]
        public void PublishToBroker_WithNullPactFileUri_ThrowArgumentNullException()
        {
            var pactPublisher = GetSubject("http://test.pact.dius.com.au");

            Assert.Throws<ArgumentNullException>(() => pactPublisher.PublishToBroker(null, "1.0.0"));
        }

        [Fact]
        public void PublishToBroker_WithEmptyPactFileUri_ThrowArgumentNullException()
        {
            var pactPublisher = GetSubject("http://test.pact.dius.com.au");

            Assert.Throws<ArgumentNullException>(() => pactPublisher.PublishToBroker(String.Empty, "1.0.0"));
        }

        [Fact]
        public void PublishToBroker_WithNullConsumerVersion_ThrowArgumentNullException()
        {
            var pactPublisher = GetSubject("http://test.pact.dius.com.au");

            Assert.Throws<ArgumentNullException>(() => pactPublisher.PublishToBroker("..\\..\\..\\Samples\\EventApi\\Consumer.Tests\\pacts\\event_api_consumer-event_api.json", null));
        }

        [Fact]
        public void PublishToBroker_WithEmptyConsumerVersion_ThrowArgumentNullException()
        {
            var pactPublisher = GetSubject("http://test.pact.dius.com.au");

            Assert.Throws<ArgumentNullException>(() => pactPublisher.PublishToBroker("..\\..\\..\\Samples\\EventApi\\Consumer.Tests\\pacts\\event_api_consumer-event_api.json", String.Empty));
        }

        [Fact]
        public void PublishToBroker_WithNoAuthentication_PublishesPact()
        {
            var brokerBaseUri = "http://test.pact.dius.com.au";
            var pactPublisher = GetSubject(brokerBaseUri);
            var pactFilePath = "..\\..\\..\\Samples\\EventApi\\Consumer.Tests\\pacts\\event_api_consumer-event_api.json";
            var pactVersion = "1.0.2";
            var pactFileText = File.ReadAllText(pactFilePath);
            var pactDetails = JsonConvert.DeserializeObject<PactDetails>(pactFileText);

            pactPublisher.PublishToBroker(pactFilePath, pactVersion);

            var requestsReceived = _fakeHttpMessageHandler.RequestsReceived;
            Assert.Equal(1, requestsReceived.Count());
            Assert.True(ValidPactPublishRequest(requestsReceived.ElementAt(0), _fakeHttpMessageHandler.RequestContentReceived.ElementAt(0), brokerBaseUri, pactDetails, pactFileText, pactVersion));
        }

        [Fact]
        public void PublishToBroker_WithAuthentication_PublishesPact()
        {
            var brokerBaseUri = "https://test.pact.dius.com.au";
            var pactUriOptions = new PactUriOptions("username", "password");
            var pactPublisher = GetSubject(brokerBaseUri, pactUriOptions);
            var pactFilePath = "..\\..\\..\\Samples\\EventApi\\Consumer.Tests\\pacts\\event_api_consumer-event_api.json";
            var pactVersion = "1.0.2";
            var pactFileText = File.ReadAllText(pactFilePath);
            var pactDetails = JsonConvert.DeserializeObject<PactDetails>(pactFileText);

            pactPublisher.PublishToBroker(pactFilePath, pactVersion);

            var requestsReceived = _fakeHttpMessageHandler.RequestsReceived;
            Assert.Equal(1, requestsReceived.Count());
            Assert.True(ValidPactPublishRequest(requestsReceived.ElementAt(0), _fakeHttpMessageHandler.RequestContentReceived.ElementAt(0), brokerBaseUri, pactDetails, pactFileText, pactVersion, pactUriOptions));
        }

        [Fact]
        public void PublishToBroker_WhenCalledWithoutTags_PublishesPactWithoutTags()
        {
            var brokerBaseUri = "https://test.pact.dius.com.au";
            var pactPublisher = GetSubject(brokerBaseUri);
            var pactFilePath = "..\\..\\..\\Samples\\EventApi\\Consumer.Tests\\pacts\\event_api_consumer-event_api.json";
            var pactVersion = "1.0.2";
            var pactFileText = File.ReadAllText(pactFilePath);
            var pactDetails = JsonConvert.DeserializeObject<PactDetails>(pactFileText);

            pactPublisher.PublishToBroker(pactFilePath, pactVersion);

            var requestsReceived = _fakeHttpMessageHandler.RequestsReceived;
            Assert.Equal(1, requestsReceived.Count());
            Assert.True(ValidPactPublishRequest(requestsReceived.ElementAt(0), _fakeHttpMessageHandler.RequestContentReceived.ElementAt(0), brokerBaseUri, pactDetails, pactFileText, pactVersion));
        }

        [Fact]
        public void PublishToBroker_WhenCalledWithTags_PublishesPactWithTags()
        {
            var brokerBaseUri = "https://test.pact.dius.com.au";
            var pactUriOptions = new PactUriOptions("username", "password");
            var pactPublisher = GetSubject(brokerBaseUri, pactUriOptions);
            var pactFilePath = "..\\..\\..\\Samples\\EventApi\\Consumer.Tests\\pacts\\event_api_consumer-event_api.json";
            var pactVersion = "1.0.2";
            var pactFileText = File.ReadAllText(pactFilePath);
            var tags = new[] { "master", "something-else" };
            var pactDetails = JsonConvert.DeserializeObject<PactDetails>(pactFileText);

            pactPublisher.PublishToBroker(pactFilePath, pactVersion, tags);

            var requestsReceived = _fakeHttpMessageHandler.RequestsReceived;
            Assert.Equal(3, requestsReceived.Count());
            Assert.True(ValidPactTagRequest(requestsReceived.ElementAt(0), _fakeHttpMessageHandler.RequestContentReceived.ElementAt(0), brokerBaseUri, pactDetails, pactVersion, tags.ElementAt(0), pactUriOptions));
            Assert.True(ValidPactTagRequest(requestsReceived.ElementAt(1), _fakeHttpMessageHandler.RequestContentReceived.ElementAt(1), brokerBaseUri, pactDetails, pactVersion, tags.ElementAt(1), pactUriOptions));
            Assert.True(ValidPactPublishRequest(requestsReceived.ElementAt(2), _fakeHttpMessageHandler.RequestContentReceived.ElementAt(2), brokerBaseUri, pactDetails, pactFileText, pactVersion, pactUriOptions));
        }

        private bool ValidPactPublishRequest(
            HttpRequestMessage httpRequest,
            string httpRequestContent,
            string brokerBaseUri,
            PactDetails expectedPactDetails,
            string expectedPactFile,
            string expectedConsumerVersion,
            PactUriOptions expectedPactUriOptions = null)
        {
            var authHeadersCorrect = false;

            if (expectedPactUriOptions != null)
            {
                if (httpRequest.Headers.Authorization.Scheme == expectedPactUriOptions.AuthorizationScheme &&
                    httpRequest.Headers.Authorization.Parameter == expectedPactUriOptions.AuthorizationValue)
                {
                    authHeadersCorrect = true;
                }
            }
            else if (httpRequest.Headers.Authorization == null)
            {
                authHeadersCorrect = true;
            }

            if (httpRequest.Method == HttpMethod.Put &&
                httpRequest.RequestUri.OriginalString == $"{brokerBaseUri}/pacts/provider/{Uri.EscapeDataString(expectedPactDetails.Provider.Name)}/consumer/{Uri.EscapeDataString(expectedPactDetails.Consumer.Name)}/version/{expectedConsumerVersion}" &&
                httpRequestContent == expectedPactFile &&
                httpRequest.Content.Headers.ContentType.MediaType == "application/json" &&
                httpRequest.Content.Headers.ContentType.CharSet == "utf-8" &&
                authHeadersCorrect)
            {
                return true;
            }

            return false;
        }

        private bool ValidPactTagRequest(
            HttpRequestMessage httpRequest,
            string httpRequestContent,
            string brokerBaseUri,
            PactDetails expectedPactDetails,
            string expectedConsumerVersion,
            string expectedTag,
            PactUriOptions expectedPactUriOptions = null)
        {
            var authHeadersCorrect = false;

            if (expectedPactUriOptions != null)
            {
                if (httpRequest.Headers.Authorization.Scheme == expectedPactUriOptions.AuthorizationScheme &&
                    httpRequest.Headers.Authorization.Parameter == expectedPactUriOptions.AuthorizationValue)
                {
                    authHeadersCorrect = true;
                }
            }
            else if (httpRequest.Headers.Authorization == null)
            {
                authHeadersCorrect = true;
            }

            if (httpRequest.Method == HttpMethod.Put &&
                httpRequest.RequestUri.OriginalString == $"{brokerBaseUri}/pacticipants/{Uri.EscapeDataString(expectedPactDetails.Consumer.Name)}/versions/{expectedConsumerVersion}/tags/{Uri.EscapeDataString(expectedTag)}" &&
                httpRequestContent == "" &&
                httpRequest.Content.Headers.ContentType.MediaType == "application/json" &&
                httpRequest.Content.Headers.ContentType.CharSet == "utf-8" &&
                authHeadersCorrect)
            {
                return true;
            }

            return false;
        }
    }
}
