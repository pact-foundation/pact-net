using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Nancy;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Nancy
{
    public class NancyContextExtensionsTests
    {
        private const string PactMockRequestResponsePairsKey = "PactMockRequestResponsePairs";

        [Fact]
        public void SetMockRequestResponsePairs_WithRequestResponsePairsAndNoExistingItemsInNancyContext_AddsRequestResponsePairsToNancyContextItems()
        {
            var context = new NancyContext();
            
            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest(), new PactProviderServiceResponse())
            };

            context.SetMockRequestResponsePairs(requestResponsePairs);

            Assert.Equal(1, context.Items.Count);
            Assert.Equal(requestResponsePairs, context.Items[PactMockRequestResponsePairsKey]);
        }

        [Fact]
        public void SetMockRequestResponsePairs_WithRequestResponsePairsAndExistingItemsInNancyContext_AddsRequestResponsePairsToNancyContextItems()
        {
            var context = new NancyContext();
            context.Items.Add(new KeyValuePair<string, object>("test", "tester"));
            
            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest(), new PactProviderServiceResponse()),
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest(), new PactProviderServiceResponse())
            };

            context.SetMockRequestResponsePairs(requestResponsePairs);

            Assert.Equal(2, context.Items.Count);
            Assert.Equal(requestResponsePairs, context.Items[PactMockRequestResponsePairsKey]);
        }

        [Fact]
        public void SetMockRequestResponsePairs_WithRequestResponsePairsAndExistingRequestResponsePairsInNancyContext_OverwritesRequestResponsePairsInNancyContextItem()
        {
            var context = new NancyContext();
            context.Items.Add(new KeyValuePair<string, object>(PactMockRequestResponsePairsKey, new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest(), new PactProviderServiceResponse()),
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest(), new PactProviderServiceResponse())
            }));

            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest(), new PactProviderServiceResponse()),
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest(), new PactProviderServiceResponse())
            };

            context.SetMockRequestResponsePairs(requestResponsePairs);

            Assert.Equal(1, context.Items.Count);
            Assert.Equal(requestResponsePairs, context.Items[PactMockRequestResponsePairsKey]);
        }

        [Fact]
        public void GetMatchingMockRequestResponsePair_WithNoRequestResponsePairs_ThrowsInvalidOperationException()
        {
            var context = new NancyContext();

            Assert.Throws<InvalidOperationException>(() => context.GetMatchingMockRequestResponsePair(HttpVerb.Get, "/events"));
        }

        [Fact]
        public void GetMatchingMockRequestResponsePair_WithNoMatchingRequestResponsePair_ThrowsArgumentException()
        {
            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest { Method = HttpVerb.Get, Path = "/hello" }, new PactProviderServiceResponse())
            }; 

            var context = new NancyContext();
            context.SetMockRequestResponsePairs(requestResponsePairs);

            Assert.Throws<ArgumentException>(() => context.GetMatchingMockRequestResponsePair(HttpVerb.Get, "/events"));
        }

        [Fact]
        public void GetMatchingMockRequestResponsePair_WithMoreThanOneMatchingRequestResponsePair_ThrowsArgumentException()
        {
            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest { Method = HttpVerb.Get, Path = "/events" }, new PactProviderServiceResponse()),
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest { Method = HttpVerb.Get, Path = "/events" }, new PactProviderServiceResponse()),
            };

            var context = new NancyContext();
            context.SetMockRequestResponsePairs(requestResponsePairs);

            Assert.Throws<ArgumentException>(() => context.GetMatchingMockRequestResponsePair(HttpVerb.Get, "/events"));
        }

        [Fact]
        public void GetMatchingMockRequestResponsePair_WithOneMatchingRequestResponsePair_ReturnsRequestResponsePair()
        {
            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest { Method = HttpVerb.Get, Path = "/events" }, new PactProviderServiceResponse()),
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest { Method = HttpVerb.Post, Path = "/events" }, new PactProviderServiceResponse()),
            };

            var context = new NancyContext();
            context.SetMockRequestResponsePairs(requestResponsePairs);

            var result = context.GetMatchingMockRequestResponsePair(HttpVerb.Get, "/events");

            Assert.Equal(requestResponsePairs.First(), result);
        }
    }
}
