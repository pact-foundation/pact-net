using System;
using System.Linq;
using System.Threading;
using NSubstitute;
using Nancy;
using Nancy.Routing;
using PactNet.Mocks.MockHttpService;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService
{
    public class MockProviderNancyRequestDispatcherTests
    {
        [Fact]
        public void Dispatch_WithNancyContext_CallsRequestHandlerWithContext()
        {
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var mockRequestHandler = Substitute.For<IMockProviderNancyRequestHandler>();
            mockRequestHandler.Handle(nancyContext).Returns(new Response());

            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(mockRequestHandler);

            requestDispatcher.Dispatch(nancyContext, CancellationToken.None);

            mockRequestHandler.Received(1).Handle(nancyContext);
        }

        [Fact]
        public void Dispatch_WithNullNancyContext_ArgumentExceptionIsSetOnTask()
        {
            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(null);

            var response = requestDispatcher.Dispatch(null, CancellationToken.None);

            Assert.Equal(typeof(ArgumentException), response.Exception.InnerExceptions.First().GetType());
        }

        [Fact]
        public void Dispatch_WithNancyContext_SetsContextResponse()
        {
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var nancyResponse = new Response
            {
                StatusCode = HttpStatusCode.OK
            };

            var mockRequestHandler = Substitute.For<IMockProviderNancyRequestHandler>();
            mockRequestHandler.Handle(nancyContext).Returns(nancyResponse);

            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(mockRequestHandler);

            requestDispatcher.Dispatch(nancyContext, CancellationToken.None);

            Assert.Equal(nancyResponse, nancyContext.Response);
        }

        [Fact]
        public void Dispatch_WithNancyContext_ReturnsResponse()
        {
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var nancyResponse = new Response
            {
                StatusCode = HttpStatusCode.OK
            };

            var mockRequestHandler = Substitute.For<IMockProviderNancyRequestHandler>();
            mockRequestHandler.Handle(nancyContext).Returns(nancyResponse);

            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(mockRequestHandler);

            var response = requestDispatcher.Dispatch(nancyContext, CancellationToken.None);

            Assert.Equal(nancyResponse, response.Result);
        }

        [Fact]
        public void Dispatch_WithNancyContext_NoExceptionIsSetOnTask()
        {
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var nancyResponse = new Response
            {
                StatusCode = HttpStatusCode.OK
            };

            var mockRequestHandler = Substitute.For<IMockProviderNancyRequestHandler>();
            mockRequestHandler.Handle(nancyContext).Returns(nancyResponse);

            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(mockRequestHandler);

            var response = requestDispatcher.Dispatch(nancyContext, CancellationToken.None);

            Assert.Null(response.Exception);
        }

        [Fact]
        public void Dispatch_WithCanceledCancellationToken_OperationCanceledExceptionIsSetOnTask()
        {
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var nancyResponse = new Response
            {
                StatusCode = HttpStatusCode.OK
            };

            var cancellationToken = new CancellationToken(true);

            var mockRequestHandler = Substitute.For<IMockProviderNancyRequestHandler>();
            mockRequestHandler.Handle(nancyContext).Returns(nancyResponse);

            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(mockRequestHandler);

            var response = requestDispatcher.Dispatch(nancyContext, cancellationToken);

            Assert.Equal(typeof(OperationCanceledException), response.Exception.InnerExceptions.First().GetType());
        }
    }
}
