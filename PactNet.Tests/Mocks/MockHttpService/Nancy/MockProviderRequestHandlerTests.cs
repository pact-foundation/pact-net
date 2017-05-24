using System;
using System.Net;
using NSubstitute;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Nancy
{
    public class MockProviderRequestHandlerTests
    {
        private IProviderServiceRequestMapper _mockRequestMapper;
        private IResponseMapper _mockResponseMapper;
        private IMockProviderRepository _mockProviderRepository;
        private ILog _mockLog;

        private IMockProviderRequestHandler GetSubject()
        {
            _mockRequestMapper = Substitute.For<IProviderServiceRequestMapper>();
            _mockResponseMapper = Substitute.For<IResponseMapper>();
            _mockProviderRepository = Substitute.For<IMockProviderRepository>();
            _mockLog = Substitute.For<ILog>();

            _mockLog.Log(Arg.Any<LogLevel>(), Arg.Any<Func<string>>(), Arg.Any<Exception>(), Arg.Any<object[]>())
                .Returns(true);

            return new MockProviderRequestHandler(_mockRequestMapper, _mockResponseMapper, _mockProviderRepository, _mockLog);
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
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/");
            
            var handler = GetSubject();

            _mockRequestMapper.Convert(request).Returns(expectedRequest);

            var interaction = new ProviderServiceInteraction { Request = expectedRequest, Response = expectedResponse };

            _mockProviderRepository.GetMatchingTestScopedInteraction(expectedRequest)
                .Returns(interaction);

            handler.Handle(request);

            _mockRequestMapper.Received(1).Convert(request);
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
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/");

            var interaction = new ProviderServiceInteraction { Request = expectedRequest, Response = expectedResponse };

            _mockProviderRepository.GetMatchingTestScopedInteraction(Arg.Any<ProviderServiceRequest>())
                .Returns(interaction);

            _mockRequestMapper.Convert(request).Returns(actualRequest);

            handler.Handle(request);

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
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/");

            var handler = GetSubject();

            _mockRequestMapper.Convert(request).Returns(expectedRequest);

            var interaction = new ProviderServiceInteraction { Request = expectedRequest, Response = expectedResponse };

            _mockProviderRepository.GetMatchingTestScopedInteraction(expectedRequest)
                .Returns(interaction);

            handler.Handle(request);

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
            var nancyResponse = new ResponseWrapper { StatusCode = HttpStatusCode.OK };

            var handler = GetSubject();
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/Test");

            var interaction = new ProviderServiceInteraction { Request = expectedRequest, Response = expectedResponse };

            _mockProviderRepository.GetMatchingTestScopedInteraction(Arg.Any<ProviderServiceRequest>())
                .Returns(interaction);

            _mockRequestMapper.Convert(request).Returns(actualRequest);
            //mockRequestComparer.Compare Doesnt throw any exceptions
            _mockResponseMapper.Convert(expectedResponse).Returns(nancyResponse);

            var response = handler.Handle(request);

            Assert.Equal(nancyResponse, response);
        }

        [Fact]
        public void Handle_WhenExceptionIsThrownHandlingRequest_PactFailureExceptionIsThrown()
        {
            var compareException = new PactFailureException("Something\r\n \t \\ failed");
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/Test");
            
            var handler = GetSubject();

            _mockRequestMapper
                .When(x => x.Convert(Arg.Any<IRequestWrapper>()))
                .Do(x => { throw compareException; });

            _mockResponseMapper.Convert(Arg.Any<ProviderServiceResponse>())
                .Returns(new ResponseWrapper
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            Assert.Throws<PactFailureException>(() => handler.Handle(request));
        }

        [Fact]
        public void Handle_WhenNoMatchingInteractionsAreFound_RequestIsMarkedAsHandled()
        {
            const string exceptionMessage = "No matching mock interaction has been registered for the current request";
            var expectedRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/Test"
            };
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/Test");

            var handler = GetSubject();

            _mockRequestMapper
                .Convert(request)
                .Returns(expectedRequest);

            _mockResponseMapper.Convert(Arg.Any<ProviderServiceResponse>())
                .Returns(new ResponseWrapper
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            _mockProviderRepository
                .When(x => x.GetMatchingTestScopedInteraction(expectedRequest))
                .Do(x => { throw new PactFailureException(exceptionMessage); });

            try
            {
                handler.Handle(request);
            }
            catch (Exception)
            {
            }

            _mockProviderRepository.Received(1).AddHandledRequest(Arg.Is<HandledRequest>(x => x.ActualRequest == expectedRequest && x.MatchedInteraction == null));
        }

        [Fact]
        public void Handle_WhenNoMatchingInteractionsAreFound_ErrorIsLogged()
        {
            const string exceptionMessage = "No matching mock interaction has been registered for the current request";
            var expectedRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/Test"
            };
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/Test");

            var handler = GetSubject();

            _mockRequestMapper
                .Convert(request)
                .Returns(expectedRequest);

            _mockResponseMapper.Convert(Arg.Any<ProviderServiceResponse>())
                .Returns(new ResponseWrapper
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            _mockProviderRepository
                .When(x => x.GetMatchingTestScopedInteraction(expectedRequest))
                .Do(x => { throw new PactFailureException(exceptionMessage); });

            try
            {
                handler.Handle(request);
            }
            catch (Exception)
            {
            }

            _mockLog.Received().Log(LogLevel.Error, Arg.Any<Func<string>>(), null, Arg.Any<object[]>());
        }

        [Fact]
        public void Handle_WhenNoMatchingInteractionsAreFound_PactFailureExceptionIsThrown()
        {
            const string exceptionMessage = "No matching mock interaction has been registered for the current request";
            var expectedRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/Test"
            };
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/Test");

            var handler = GetSubject();

            _mockRequestMapper
                .Convert(request)
                .Returns(expectedRequest);

            _mockResponseMapper.Convert(Arg.Any<ProviderServiceResponse>())
                .Returns(new ResponseWrapper
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            _mockProviderRepository
                .When(x => x.GetMatchingTestScopedInteraction(expectedRequest))
                .Do(x => { throw new PactFailureException(exceptionMessage); });

            Assert.Throws<PactFailureException>(() => handler.Handle(request));
        }
    }
}
