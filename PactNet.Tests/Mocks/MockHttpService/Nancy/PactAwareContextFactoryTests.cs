using System.Collections.Generic;
using NSubstitute;
using Nancy;
using Nancy.Culture;
using Nancy.Diagnostics;
using Nancy.Localization;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Nancy;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Nancy
{
    public class PactAwareContextFactoryTests
    {
        [Fact]
        public void Create_WithRequest_SetRequestOnContext()
        {
            var request = new Request("GET", "/events", "HTTP");

            var mockMockContextService = Substitute.For<IMockContextService>();
            var mockCultureService = Substitute.For<ICultureService>();
            var mockRequestTraceFactory = Substitute.For<IRequestTraceFactory>();
            var mockTextResource = Substitute.For<ITextResource>();

            INancyContextFactory nancyContextFactory = new PactAwareContextFactory(
                mockMockContextService,
                mockCultureService,
                mockRequestTraceFactory,
                mockTextResource);

            var context = nancyContextFactory.Create(request);

            Assert.Equal(request, context.Request);
        }

        [Fact]
        public void Create_WithRequest_CallsMockContentServiceAndAssignsRequestResponsePairsOnNancyContextItem()
        {
            var request = new Request("GET", "/events", "HTTP");
            var requestResponsePairs = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction() { Request = new ProviderServiceRequest { Method = HttpVerb.Get, Path = "/events" }, Response = new ProviderServiceResponse() },
                new ProviderServiceInteraction() { Request = new ProviderServiceRequest { Method = HttpVerb.Post, Path = "/events" }, Response = new ProviderServiceResponse() },
            };

            var mockMockContextService = Substitute.For<IMockContextService>();
            var mockCultureService = Substitute.For<ICultureService>();
            var mockRequestTraceFactory = Substitute.For<IRequestTraceFactory>();
            var mockTextResource = Substitute.For<ITextResource>();

            mockMockContextService.GetExpectedRequestResponsePairs().Returns(requestResponsePairs);

            INancyContextFactory nancyContextFactory = new PactAwareContextFactory(
                mockMockContextService,
                mockCultureService,
                mockRequestTraceFactory,
                mockTextResource);

            var context = nancyContextFactory.Create(request);

            Assert.Equal(requestResponsePairs, context.Items["PactMockInteractions"]);
            mockMockContextService.Received(1).GetExpectedRequestResponsePairs();
        }
    }
}
