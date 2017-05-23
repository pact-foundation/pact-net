#if USE_NANCY
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NSubstitute;
using Nancy;
using Nancy.Routing;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Nancy;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Nancy
{
    public class MockProviderNancyRequestDispatcherTests
    {
        private IMockProviderRequestHandler _mockRequestHandler;
        private IMockProviderAdminRequestHandler _mockAdminRequestHandler;
        private ILog _log;

        private IRequestDispatcher GetSubject()
        {
            _mockRequestHandler = Substitute.For<IMockProviderRequestHandler>();
            _mockAdminRequestHandler = Substitute.For<IMockProviderAdminRequestHandler>();
            _log = Substitute.For<ILog>();

            return new MockProviderNancyRequestDispatcher(_mockRequestHandler, _mockAdminRequestHandler, _log, new PactConfig());
        }

        [Fact]
        public void Dispatch_WithNancyContext_CallsRequestHandlerWithContext()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/Test");
            
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var requestDispatcher = GetSubject();

            _mockRequestHandler.Handle(request).Returns(new ResponseWrapper());

            requestDispatcher.Dispatch(nancyContext, CancellationToken.None);

            _mockRequestHandler.Received(1).Handle(
                Arg.Is<NancyRequest>(nr => nr.Method == request.Method && nr.Path == request.Path));
        }

        [Fact]
        public void Dispatch_WithNancyContextThatContainsAdminHeader_CallsAdminRequestHandlerWithContext()
        {
            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { Constants.AdministrativeRequestHeaderKey, new List<string> { "true" } }
            };

            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/Test");
            request.Headers.Returns(headers);

            var nancyContext = new NancyContext
            {
                Request = new Request("GET", new Url
                {
                    Path = "/Test",
                    Scheme = "HTTP"
                }, null, headers)
            };

            var requestDispatcher = GetSubject();

            _mockAdminRequestHandler.Handle(request).Returns(new ResponseWrapper());

            requestDispatcher.Dispatch(nancyContext, CancellationToken.None);

            _mockAdminRequestHandler.Received(1).Handle(
                Arg.Is<NancyRequest>(nr => nr.Method == request.Method && nr.Path == request.Path &&  nr.Headers.SequenceEqual(request.Headers)));
        }

        [Fact]
        public void Dispatch_WithNullNancyContext_ArgumentExceptionIsSetOnTask()
        {
            var requestDispatcher = GetSubject();

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
            
            var requestDispatcher = GetSubject();

            _mockRequestHandler.Handle(
                Arg.Is<NancyRequest>(request => request.Method == "GET" && request.Path == "/Test")).Returns(new ResponseWrapper { StatusCode = System.Net.HttpStatusCode.OK });

            requestDispatcher.Dispatch(nancyContext, CancellationToken.None);

            Assert.Equal(HttpStatusCode.OK, nancyContext.Response.StatusCode);
        }

        [Fact]
        public void Dispatch_WithNancyContext_ReturnsResponse()
        {
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var requestDispatcher = GetSubject();

            _mockRequestHandler.Handle(
                Arg.Is<NancyRequest>(request => request.Method == "GET" && request.Path == "/Test")).Returns(new ResponseWrapper { StatusCode = System.Net.HttpStatusCode.OK });

            var response = requestDispatcher.Dispatch(nancyContext, CancellationToken.None);

            Assert.Equal(HttpStatusCode.OK, response.Result.StatusCode);
        }

        [Fact]
        public void Dispatch_WithNancyContext_NoExceptionIsSetOnTask()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/Test");

            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var requestDispatcher = GetSubject();

            _mockRequestHandler.Handle(request).Returns(new ResponseWrapper { StatusCode = System.Net.HttpStatusCode.OK });

            var response = requestDispatcher.Dispatch(nancyContext, CancellationToken.None);

            Assert.Null(response.Exception);
        }

        [Fact]
        public void Dispatch_WithCanceledCancellationToken_OperationCanceledExceptionIsSetOnTask()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/Test");
            
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var cancellationToken = new CancellationToken(true);

            var requestDispatcher = GetSubject();

            _mockRequestHandler.Handle(request).Returns(new ResponseWrapper { StatusCode = System.Net.HttpStatusCode.OK });

            var response = requestDispatcher.Dispatch(nancyContext, cancellationToken);

            Assert.Equal(typeof(OperationCanceledException), response.Exception.InnerExceptions.First().GetType());
        }

        [Fact]
        public void Dispatch_WhenRequestHandlerThrows_InternalServerErrorResponseIsReturned()
        {
            var exception = new InvalidOperationException("Something failed.");
            const string expectedMessage = "Something failed. See logs for details.";
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var requestDispatcher = GetSubject();

            _mockRequestHandler
                .When(x => x.Handle(Arg.Any<IRequestWrapper>()))
                .Do(x => { throw exception; });
            
            var response = requestDispatcher.Dispatch(nancyContext, CancellationToken.None).Result;

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(expectedMessage, response.ReasonPhrase);
            Assert.Equal(expectedMessage, ReadResponseContent(response));
        }

        [Fact]
        public void Dispatch_WhenRequestHandlerThrowsWithMessageThatContainsSlashes_ResponseContentAndReasonPhrasesIsReturnedWithoutSlashes()
        {
            var exception = new InvalidOperationException("Something\r\n \t \\ failed.");
            const string expectedMessage = @"Something\r\n \t \\ failed. See logs for details.";
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var requestDispatcher = GetSubject();

            _mockRequestHandler
                .When(x => x.Handle(Arg.Any<IRequestWrapper>()))
                .Do(x => { throw exception; });

            var response = requestDispatcher.Dispatch(nancyContext, CancellationToken.None).Result;

            Assert.Equal(expectedMessage, response.ReasonPhrase);
            Assert.Equal(expectedMessage, ReadResponseContent(response));
        }

        [Fact]
        public void Dispatch_WhenRequestHandlerThrows_TheExceptionIsLogged()
        {
            var exception = new InvalidOperationException("Something failed.");
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var requestDispatcher = GetSubject();

            _mockRequestHandler
                .When(x => x.Handle(Arg.Any<IRequestWrapper>()))
                .Do(x => { throw exception; });

            var response = requestDispatcher.Dispatch(nancyContext, CancellationToken.None).Result;

            _log.Received(1).ErrorException(Arg.Any<string>(), exception);
        }

        [Fact]
        public void Dispatch_WhenRequestHandlerThrowsAPactFailureException_TheExceptionIsNotLogged()
        {
            var exception = new PactFailureException("Something failed");
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var requestDispatcher = GetSubject();

            _mockRequestHandler
                .When(x => x.Handle(Arg.Any<IRequestWrapper>()))
                .Do(x => { throw exception; });

            var response = requestDispatcher.Dispatch(nancyContext, CancellationToken.None).Result;

            _log.DidNotReceive().ErrorException(Arg.Any<string>(), Arg.Any<Exception>());
        }

        private string ReadResponseContent(Response response)
        {
            string content;
            using (var stream = new MemoryStream())
            {
                response.Contents(stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }

            return content;
        }
    }
}
#endif