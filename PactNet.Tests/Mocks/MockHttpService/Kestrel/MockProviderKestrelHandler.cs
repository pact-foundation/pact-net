#if USE_KESTREL

using Microsoft.AspNetCore.Http;
using NSubstitute;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Kestrel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Kestrel
{
    public class MockProviderKestrelHandlerTests
    {
        private IMockProviderRequestHandler _mockRequestHandler;
        private IMockProviderAdminRequestHandler _mockAdminRequestHandler;
        private ILog _log;

        private KestrelHandler GetSubject()
        {
            _mockRequestHandler = Substitute.For<IMockProviderRequestHandler>();
            _mockAdminRequestHandler = Substitute.For<IMockProviderAdminRequestHandler>();
            _log = Substitute.For<ILog>();

            return new KestrelHandler(context => Task.CompletedTask, _mockRequestHandler, _mockAdminRequestHandler, _log, new PactConfig());
        }

        [Fact]
        public async Task Dispatch_WithHttpContext_CallsRequestHandlerWithContext()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/Test");

            var httpContext = new DefaultHttpContext();

            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/Test";

            KestrelHandler handler = GetSubject();

            _mockRequestHandler.Handle(request).Returns(new ResponseWrapper());

            await handler.Invoke(httpContext);

            _mockRequestHandler.Received(1).Handle(
                Arg.Is<KestrelRequest>(nr => nr.Method == request.Method && nr.Path == request.Path));
        }

        [Fact]
        public async Task Dispatch_WithHttpContextThatContainsAdminHeader_CallsAdminRequestHandlerWithContext()
        {
            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { Constants.AdministrativeRequestHeaderKey, new List<string> { "true" } }
            };

            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/Test");
            request.Headers.Returns(headers);

            var httpContext = new DefaultHttpContext();

            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/Test";
            AddHeaders(headers, httpContext.Request.Headers);

            KestrelHandler handler = GetSubject();

            _mockAdminRequestHandler.Handle(request).Returns(new ResponseWrapper());

            await handler.Invoke(httpContext);

            _mockAdminRequestHandler.Received(1).Handle(
                Arg.Is<KestrelRequest>(nr => nr.Method == request.Method && nr.Path == request.Path && CompareHeaders(nr.Headers, request.Headers)));
        }

        [Fact]
        public async Task Dispatch_WithHttpContext_ReturnsResponse()
        {
            var httpContext = Substitute.For<HttpContext>();

            httpContext.Request.Body.Returns(Stream.Null);
            httpContext.Request.Method.Returns("GET");
            httpContext.Request.Path.Returns(new PathString("/Test"));
            httpContext.Response.Body.Returns(new MemoryStream());

            KestrelHandler handler = GetSubject();

            _mockRequestHandler.Handle(
                Arg.Is<KestrelRequest>(request => request.Method == "GET" && request.Path == "/Test")).Returns(new ResponseWrapper { StatusCode = HttpStatusCode.OK });

            await handler.Invoke(httpContext);

            Assert.Equal((int)HttpStatusCode.OK, httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task Dispatch_WhenRequestHandlerThrows_InternalServerErrorResponseIsReturned()
        {
            var exception = new InvalidOperationException("Something failed.");
            const string expectedMessage = "Something failed. See logs for details.";
            var httpContext = Substitute.For<HttpContext>();

            httpContext.Request.Body.Returns(Stream.Null);
            httpContext.Request.Method.Returns("GET");
            httpContext.Request.Path.Returns(new PathString("/Test"));
            httpContext.Response.Body.Returns(new MemoryStream());

            KestrelHandler handler = GetSubject();

            _mockRequestHandler
                .When(x => x.Handle(Arg.Any<IRequestWrapper>()))
                .Do(x => throw exception);

            await handler.Invoke(httpContext);

            Assert.Equal((int)HttpStatusCode.InternalServerError, httpContext.Response.StatusCode);
            Assert.Equal(expectedMessage, ReadResponseContent(httpContext.Response.Body));
        }

        [Fact]
        public async Task Dispatch_WhenRequestHandlerThrowsWithMessageThatContainsSlashes_ResponseContentAndReasonPhrasesIsReturnedWithoutSlashes()
        {
            var exception = new InvalidOperationException("Something\r\n \t \\ failed.");
            const string expectedMessage = @"Something\r\n \t \\ failed. See logs for details.";
            var httpContext = Substitute.For<HttpContext>();

            httpContext.Request.Body.Returns(Stream.Null);
            httpContext.Request.Method.Returns("GET");
            httpContext.Request.Path.Returns(new PathString("/Test"));
            httpContext.Response.Body.Returns(new MemoryStream());

            KestrelHandler handler = GetSubject();

            _mockRequestHandler
                .When(x => x.Handle(Arg.Any<IRequestWrapper>()))
                .Do(x => throw exception);

            await handler.Invoke(httpContext);

            Assert.Equal(expectedMessage, ReadResponseContent(httpContext.Response.Body));
        }

        [Fact]
        public async Task Dispatch_WhenRequestHandlerThrows_TheExceptionIsLogged()
        {
            var exception = new InvalidOperationException("Something failed.");
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/Test";

            KestrelHandler handler = GetSubject();

            _mockRequestHandler
                .When(x => x.Handle(Arg.Any<IRequestWrapper>()))
                .Do(x => throw exception);

            await handler.Invoke(httpContext);

            _log.Received(1).ErrorException(Arg.Any<string>(), exception);
        }

        [Fact]
        public async Task Dispatch_WhenRequestHandlerThrowsAPactFailureException_TheExceptionIsNotLogged()
        {
            var exception = new PactFailureException("Something failed");
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Method = "GET";
            httpContext.Request.Path = "/Test";

            KestrelHandler handler = GetSubject();

            _mockRequestHandler
                .When(x => x.Handle(Arg.Any<IRequestWrapper>()))
                .Do(x => throw exception);

            await handler.Invoke(httpContext);

            _log.DidNotReceive().ErrorException(Arg.Any<string>(), Arg.Any<Exception>());
        }

        private string ReadResponseContent(Stream response)
        {
            var content = new byte[response.Length];

            response.Position = 0;
            response.ReadAsync(content, 0, content.Length);

            return Encoding.UTF8.GetString(content);
        }

        private void AddHeaders(IDictionary<string, IEnumerable<string>> source, IHeaderDictionary destination)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> header in source)
            {
                destination.Add(header.Key, header.Value.ToArray());
            }
        }

        private bool CompareHeaders(IDictionary<string, IEnumerable<string>> source, IDictionary<string, IEnumerable<string>> destination)
        {
            return source.Keys.Count == destination.Keys.Count &&
                !source.Keys.Except(destination.Keys).Any() &&
                source.Keys.All(expectedKey => destination[expectedKey].SequenceEqual(source[expectedKey]));
        }
    }
}

#endif