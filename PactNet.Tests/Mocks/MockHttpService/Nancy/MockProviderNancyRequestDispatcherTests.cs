using System.Collections.Generic;
using System.Threading;
using Nancy;
using Nancy.Routing;
using NSubstitute;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Nancy;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Nancy
{
    public class MockProviderNancyRequestDispatcherTests
    {
        [Fact]
        public void Dispatch_WhenExpectedRequestHasNotBeenSet_ResponseMapperIsCalledAndReturns500Response()
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

            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(null, null, mockNancyResponseMapper);

            var result = requestDispatcher.Dispatch(nancyContext, CancellationToken.None).Result;

            mockNancyResponseMapper.Received(1).Convert(Arg.Is<PactProviderServiceResponse>(x => x.Status == 500));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public void Dispatch_WhenExpectedResponseHasNotBeenSet_ResponseMapperIsCalledAndReturns500Response()
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

            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(null, null, mockNancyResponseMapper);

            var result = requestDispatcher.Dispatch(nancyContext, CancellationToken.None).Result;

            mockNancyResponseMapper.Received(1).Convert(Arg.Is<PactProviderServiceResponse>(x => x.Status == 500));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public void Dispatch_WithCanceledCancellationToken_ResponseMapperIsCalledAndReturns500Response()
        {
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/", "HTTP")
            };

            var requestResponsePairs = new List<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>
            {
                new KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>(new PactProviderServiceRequest(), new PactProviderServiceResponse())
            };

            nancyContext.SetMockRequestResponsePairs(requestResponsePairs);

            var mockNancyResponseMapper = Substitute.For<INancyResponseMapper>();
            mockNancyResponseMapper.Convert(Arg.Any<PactProviderServiceResponse>())
                .Returns(new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(null, null, mockNancyResponseMapper);

            var result = requestDispatcher.Dispatch(nancyContext, CancellationToken.None).Result;

            mockNancyResponseMapper.Received(1).Convert(Arg.Is<PactProviderServiceResponse>(x => x.Status == 500));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public void Dispatch_WithNancyContext_ConvertIsCalledOnThePactProviderServiceRequestMapper()
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

            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(
                mockRequestComparer,
                mockRequestMapper,
                mockResponseMapper);

            requestDispatcher.Dispatch(nancyContext, CancellationToken.None);

            mockRequestMapper.Received(1).Convert(nancyContext.Request);
        }

        [Fact]
        public void Dispatch_WithNancyContext_CompareIsCalledOnThePactProviderServiceRequestComparer()
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

            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(
                mockRequestComparer,
                mockRequestMapper,
                mockResponseMapper);

            requestDispatcher.Dispatch(nancyContext, CancellationToken.None);

            mockRequestComparer.Received(1).Compare(expectedRequest, actualRequest);
        }

        [Fact]
        public void Dispatch_WithNancyContext_ConvertIsCalledOnTheNancyResponseMapper()
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

            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(
                mockRequestComparer,
                mockRequestMapper,
                mockResponseMapper);

            requestDispatcher.Dispatch(nancyContext, CancellationToken.None);

            mockResponseMapper.Received(1).Convert(expectedResponse);
        }

        [Fact]
        public void Dispatch_WithNancyContextRequestThatMatchesExpectedRequest_ReturnsNancyResponse()
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

            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(
                mockRequestComparer,
                mockRequestMapper,
                mockResponseMapper);

            var response = requestDispatcher.Dispatch(nancyContext, CancellationToken.None);

            Assert.Equal(nancyResponse, response.Result);
            Assert.Equal(nancyResponse, nancyContext.Response);
            Assert.Null(response.Exception);
        }

        [Fact]
        public void Dispatch_WithNancyContextRequestThatDoesNotMatchExpectedRequest_ResponseMapperIsCalledAndReturns500Response()
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

            IRequestDispatcher requestDispatcher = new MockProviderNancyRequestDispatcher(
                mockRequestComparer,
                mockRequestMapper,
                mockResponseMapper);

            var response = requestDispatcher.Dispatch(nancyContext, CancellationToken.None).Result;

            mockResponseMapper.Received(1).Convert(Arg.Is<PactProviderServiceResponse>(x => x.Status == 500));
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}
