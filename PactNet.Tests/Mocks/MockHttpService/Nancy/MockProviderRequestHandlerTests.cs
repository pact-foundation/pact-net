using System;
using Nancy;
using NSubstitute;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Nancy;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Nancy
{
    public class MockProviderRequestHandlerTests
    {
        private IProviderServiceRequestMapper _mockRequestMapper;
        private INancyResponseMapper _mockResponseMapper;
        private IMockProviderRepository _mockProviderRepository;
        private ILogger _mockLogger;

        private IMockProviderRequestHandler GetSubject()
        {
            _mockRequestMapper = Substitute.For<IProviderServiceRequestMapper>();
            _mockResponseMapper = Substitute.For<INancyResponseMapper>();
            _mockProviderRepository = Substitute.For<IMockProviderRepository>();
            _mockLogger = Substitute.For<ILogger>();

            return new MockProviderRequestHandler(_mockRequestMapper, _mockResponseMapper, _mockProviderRepository, _mockLogger);
        }

        [Fact]
        public void Handle_WithNancyContext_ConvertIsCalledOnThProviderServiceRequestMapper()
        {
            var expectedRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/"
            };
            var expectedResponse = new ProviderServiceResponse();
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/", "HTTP")
            };
            var handler = GetSubject();

            _mockRequestMapper.Convert(nancyContext.Request).Returns(expectedRequest);

            var interaction = new ProviderServiceInteraction { Request = expectedRequest, Response = expectedResponse };

            _mockProviderRepository.GetMatchingTestScopedInteraction(expectedRequest)
                .Returns(interaction);

            handler.Handle(nancyContext);

            _mockRequestMapper.Received(1).Convert(nancyContext.Request);
        }

        [Fact]
        public void Handle_WithNancyContext_AddHandledRequestIsCalledOnTheMockProviderRepository()
        {
            var expectedRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/"
            };
            var actualRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/",
                Body = new {}
            };
            var expectedResponse = new ProviderServiceResponse();

            var handler = GetSubject();

            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/", "HTTP")
            };

            var interaction = new ProviderServiceInteraction { Request = expectedRequest, Response = expectedResponse };

            _mockProviderRepository.GetMatchingTestScopedInteraction(Arg.Any<ProviderServiceRequest>())
                .Returns(interaction);

            _mockRequestMapper.Convert(nancyContext.Request).Returns(actualRequest);

            handler.Handle(nancyContext);

            _mockProviderRepository.Received(1).AddHandledRequest(Arg.Is<HandledRequest>(x => x.ActualRequest == actualRequest && x.MatchedInteraction == interaction));
        }

        [Fact]
        public void Handle_WithNancyContext_ConvertIsCalledOnTheNancyResponseMapper()
        {   
            var expectedRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/"
            };
            var expectedResponse = new ProviderServiceResponse();
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/", "HTTP")
            };

            var handler = GetSubject();

            _mockRequestMapper.Convert(nancyContext.Request).Returns(expectedRequest);

            var interaction = new ProviderServiceInteraction { Request = expectedRequest, Response = expectedResponse };

            _mockProviderRepository.GetMatchingTestScopedInteraction(expectedRequest)
                .Returns(interaction);

            handler.Handle(nancyContext);

            _mockResponseMapper.Received(1).Convert(expectedResponse);
        }

        [Fact]
        public void Handle_WithNancyContextRequestThatMatchesExpectedRequest_ReturnsNancyResponse()
        {
            var expectedRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/Test"
            };
            var actualRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/Test"
            };
            var expectedResponse = new ProviderServiceResponse { Status = 200 };
            var nancyResponse = new Response { StatusCode = HttpStatusCode.OK };

            var handler = GetSubject();

            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var interaction = new ProviderServiceInteraction { Request = expectedRequest, Response = expectedResponse };

            _mockProviderRepository.GetMatchingTestScopedInteraction(Arg.Any<ProviderServiceRequest>())
                .Returns(interaction);

            _mockRequestMapper.Convert(nancyContext.Request).Returns(actualRequest);
            //mockRequestComparer.Compare Doesnt throw any exceptions
            _mockResponseMapper.Convert(expectedResponse).Returns(nancyResponse);

            var response = handler.Handle(nancyContext);

            Assert.Equal(nancyResponse, response);
        }

        [Fact]
        public void Handle_WhenExceptionIsThrownHandlingRequest_PactFailureExceptionIsThrown()
        {
            var compareException = new PactFailureException("Something\r\n \t \\ failed");

            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var handler = GetSubject();

            _mockRequestMapper
                .When(x => x.Convert(Arg.Any<Request>()))
                .Do(x => { throw compareException; });

            _mockResponseMapper.Convert(Arg.Any<ProviderServiceResponse>())
                .Returns(new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            Assert.Throws<PactFailureException>(() => handler.Handle(nancyContext));
        }

        [Fact]
        public void Handle_WhenGetMatchingMockInteractionThrows_RequestIsMarkedAsHandled()
        {
            const string exceptionMessage = "No matching mock interaction has been registered for the current request";
            var expectedRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/Test"
            };
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var handler = GetSubject();

            _mockRequestMapper
                .Convert(nancyContext.Request)
                .Returns(expectedRequest);

            _mockResponseMapper.Convert(Arg.Any<ProviderServiceResponse>())
                .Returns(new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            _mockProviderRepository
                .When(x => x.GetMatchingTestScopedInteraction(expectedRequest))
                .Do(x => { throw new PactFailureException(exceptionMessage); });

            try
            {
                handler.Handle(nancyContext);
            }
            catch (Exception)
            {
            }

            _mockProviderRepository.Received(1).AddHandledRequest(Arg.Is<HandledRequest>(x => x.ActualRequest == expectedRequest && x.MatchedInteraction == null));
        }

        [Fact]
        public void Handle_WhenGetMatchingMockInteractionThrows_PactFailureExceptionIsThrown()
        {
            const string exceptionMessage = "No matching mock interaction has been registered for the current request";
            var expectedRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/Test"
            };
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var handler = GetSubject();

            _mockRequestMapper
                .Convert(nancyContext.Request)
                .Returns(expectedRequest);

            _mockResponseMapper.Convert(Arg.Any<ProviderServiceResponse>())
                .Returns(new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            _mockProviderRepository
                .When(x => x.GetMatchingTestScopedInteraction(expectedRequest))
                .Do(x => { throw new PactFailureException(exceptionMessage); });

            Assert.Throws<PactFailureException>(() => handler.Handle(nancyContext));
        }
    }
}
