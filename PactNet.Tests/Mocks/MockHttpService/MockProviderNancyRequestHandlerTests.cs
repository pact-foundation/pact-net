using System.Collections.Generic;
using Nancy;
using NSubstitute;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService
{
    public class MockProviderNancyRequestHandlerTests
    {
        [Fact]
        public void Handle_WhenExpectedRequestHasNotBeenSet_ResponseMapperIsCalledAndReturns500Response()
        {
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/", "HTTP")
            };

            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(null, new PactProviderServiceResponse())
            };

            nancyContext.SetMockRequestResponsePairs(requestResponsePairs);

            var mockNancyResponseMapper = Substitute.For<INancyResponseMapper>();
            mockNancyResponseMapper.Convert(Arg.Any<PactProviderServiceResponse>())
                .Returns(new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(null, null, mockNancyResponseMapper);

            var result = handler.Handle(nancyContext);

            mockNancyResponseMapper.Received(1).Convert(Arg.Is<PactProviderServiceResponse>(x => x.Status == 500));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public void Handle_WhenExpectedResponseHasNotBeenSet_ResponseMapperIsCalledAndReturns500Response()
        {
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/", "HTTP")
            };

            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest(), null)
            };

            nancyContext.SetMockRequestResponsePairs(requestResponsePairs);

            var mockNancyResponseMapper = Substitute.For<INancyResponseMapper>();
            mockNancyResponseMapper.Convert(Arg.Any<PactProviderServiceResponse>())
                .Returns(new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(null, null, mockNancyResponseMapper);

            var result = handler.Handle(nancyContext);

            mockNancyResponseMapper.Received(1).Convert(Arg.Is<PactProviderServiceResponse>(x => x.Status == 500));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public void Handle_WithNancyContext_ConvertIsCalledOnThePactProviderServiceRequestMapper()
        {
            var expectedRequest = new PactProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/"
            };
            var expectedResponse = new PactProviderServiceResponse();
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/", "HTTP")
            };

            var mockRequestComparer = Substitute.For<IPactProviderServiceRequestComparer>();
            var mockRequestMapper = Substitute.For<IPactProviderServiceRequestMapper>();
            var mockResponseMapper = Substitute.For<INancyResponseMapper>();

            mockRequestMapper.Convert(nancyContext.Request).Returns(expectedRequest);

            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(expectedRequest, expectedResponse)
            };

            nancyContext.SetMockRequestResponsePairs(requestResponsePairs);

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(mockRequestComparer, mockRequestMapper, mockResponseMapper);

            handler.Handle(nancyContext);

            mockRequestMapper.Received(1).Convert(nancyContext.Request);
        }

        [Fact]
        public void Handle_WithNancyContext_CompareIsCalledOnThePactProviderServiceRequestComparer()
        {   
            var expectedRequest = new PactProviderServiceRequest();
            var expectedResponse = new PactProviderServiceResponse();
            var actualRequest = new PactProviderServiceRequest();

            var mockRequestComparer = Substitute.For<IPactProviderServiceRequestComparer>();
            var mockRequestMapper = Substitute.For<IPactProviderServiceRequestMapper>();
            var mockResponseMapper = Substitute.For<INancyResponseMapper>();

            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/", "HTTP")
            };

            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(expectedRequest, expectedResponse)
            };

            nancyContext.SetMockRequestResponsePairs(requestResponsePairs);

            mockRequestMapper.Convert(nancyContext.Request).Returns(actualRequest);

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(mockRequestComparer, mockRequestMapper, mockResponseMapper);

            handler.Handle(nancyContext);

            mockRequestComparer.Received(1).Compare(expectedRequest, actualRequest);
        }

        [Fact]
        public void Handle_WithNancyContext_ConvertIsCalledOnTheNancyResponseMapper()
        {   
            var expectedRequest = new PactProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/"
            };
            var expectedResponse = new PactProviderServiceResponse();
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/", "HTTP")
            };

            var mockRequestComparer = Substitute.For<IPactProviderServiceRequestComparer>();
            var mockRequestMapper = Substitute.For<IPactProviderServiceRequestMapper>();
            var mockResponseMapper = Substitute.For<INancyResponseMapper>();

            mockRequestMapper.Convert(nancyContext.Request).Returns(expectedRequest);

            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(expectedRequest, expectedResponse)
            };

            nancyContext.SetMockRequestResponsePairs(requestResponsePairs);

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(mockRequestComparer, mockRequestMapper, mockResponseMapper);

            handler.Handle(nancyContext);

            mockResponseMapper.Received(1).Convert(expectedResponse);
        }

        [Fact]
        public void Handle_WithNancyContextRequestThatMatchesExpectedRequest_ReturnsNancyResponse()
        {
            var expectedRequest = new PactProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/Test"
            };
            var actualRequest = new PactProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/Test"
            };
            var expectedResponse = new PactProviderServiceResponse { Status = 200 };
            var nancyResponse = new Response { StatusCode = HttpStatusCode.OK };

            var mockRequestComparer = Substitute.For<IPactProviderServiceRequestComparer>();
            var mockRequestMapper = Substitute.For<IPactProviderServiceRequestMapper>();
            var mockResponseMapper = Substitute.For<INancyResponseMapper>();

            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(expectedRequest, expectedResponse)
            };

            nancyContext.SetMockRequestResponsePairs(requestResponsePairs);

            mockRequestMapper.Convert(nancyContext.Request).Returns(actualRequest);
            //mockRequestComparer.Compare Doesnt throw any exceptions
            mockResponseMapper.Convert(expectedResponse).Returns(nancyResponse);

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(mockRequestComparer, mockRequestMapper, mockResponseMapper);

            var response = handler.Handle(nancyContext);

            Assert.Equal(nancyResponse, response);
        }

        [Fact]
        public void Handle_WithNancyContextRequestThatDoesNotMatchExpectedRequest_ResponseMapperIsCalledAndReturns500Response()
        {   
            var expectedRequest = new PactProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/Test"
            };
            var actualRequest = new PactProviderServiceRequest
            {
                Method = HttpVerb.Put,
                Path = "/Test"
            };
            var expectedResponse = new PactProviderServiceResponse { Status = 200 };
            var nancyResponse = new Response { StatusCode = HttpStatusCode.OK };
            var compareException = new CompareFailedException(expectedRequest.Method, actualRequest.Method);

            var mockRequestComparer = Substitute.For<IPactProviderServiceRequestComparer>();
            var mockRequestMapper = Substitute.For<IPactProviderServiceRequestMapper>();
            var mockResponseMapper = Substitute.For<INancyResponseMapper>();

            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(expectedRequest, expectedResponse)
            };

            nancyContext.SetMockRequestResponsePairs(requestResponsePairs);

            mockRequestMapper.Convert(nancyContext.Request).Returns(actualRequest);
            mockRequestComparer
                .When(x => x.Compare(expectedRequest, actualRequest))
                .Do(x => { throw compareException; });

            mockResponseMapper.Convert(expectedResponse).Returns(nancyResponse);

            mockResponseMapper.Convert(Arg.Any<PactProviderServiceResponse>())
                .Returns(new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(mockRequestComparer, mockRequestMapper, mockResponseMapper);

            var response = handler.Handle(nancyContext);

            mockResponseMapper.Received(1).Convert(Arg.Is<PactProviderServiceResponse>(x => x.Status == 500));
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}
