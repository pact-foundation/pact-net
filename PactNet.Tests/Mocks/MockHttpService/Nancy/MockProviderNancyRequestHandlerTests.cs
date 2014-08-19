using System.Collections.Generic;
using System.IO;
using Nancy;
using NSubstitute;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Nancy;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Nancy
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

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction { Request =  null, Response = new ProviderServiceResponse()}
            };

            nancyContext.SetMockInteraction(interactions);


            var mockRequestComparer = Substitute.For<IProviderServiceRequestComparer>();
            var mockRequestMapper = Substitute.For<IProviderServiceRequestMapper>();
            var mockResponseMapper = Substitute.For<INancyResponseMapper>();

            mockResponseMapper.Convert(Arg.Any<ProviderServiceResponse>())
                .Returns(new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(mockRequestComparer, mockRequestMapper, mockResponseMapper);

            var result = handler.Handle(nancyContext);

            mockResponseMapper.Received(1).Convert(Arg.Is<ProviderServiceResponse>(x => x.Status == 500));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        [Fact]
        public void Handle_WhenExpectedResponseHasNotBeenSet_ResponseMapperIsCalledAndReturns500Response()
        {
            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/", "HTTP")
            };

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction { Request = new ProviderServiceRequest(), Response = null }
            };

            nancyContext.SetMockInteraction(interactions);

            var mockRequestComparer = Substitute.For<IProviderServiceRequestComparer>();
            var mockRequestMapper = Substitute.For<IProviderServiceRequestMapper>();
            var mockResponseMapper = Substitute.For<INancyResponseMapper>();

            mockResponseMapper.Convert(Arg.Any<ProviderServiceResponse>())
                .Returns(new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(mockRequestComparer, mockRequestMapper, mockResponseMapper);

            var result = handler.Handle(nancyContext);

            mockResponseMapper.Received(1).Convert(Arg.Is<ProviderServiceResponse>(x => x.Status == 500));
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
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

            var mockRequestComparer = Substitute.For<IProviderServiceRequestComparer>();
            var mockRequestMapper = Substitute.For<IProviderServiceRequestMapper>();
            var mockResponseMapper = Substitute.For<INancyResponseMapper>();

            mockRequestMapper.Convert(nancyContext.Request).Returns(expectedRequest);

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction { Request = expectedRequest, Response = expectedResponse}
            };

            nancyContext.SetMockInteraction(interactions);

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(mockRequestComparer, mockRequestMapper, mockResponseMapper);

            handler.Handle(nancyContext);

            mockRequestMapper.Received(1).Convert(nancyContext.Request);
        }

        [Fact]
        public void Handle_WithNancyContext_CompareIsCalledOnTheProviderServiceRequestComparer()
        {   
            var expectedRequest = new ProviderServiceRequest();
            var expectedResponse = new ProviderServiceResponse();
            var actualRequest = new ProviderServiceRequest();

            var mockRequestComparer = Substitute.For<IProviderServiceRequestComparer>();
            var mockRequestMapper = Substitute.For<IProviderServiceRequestMapper>();
            var mockResponseMapper = Substitute.For<INancyResponseMapper>();

            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/", "HTTP")
            };

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction { Request = expectedRequest, Response = expectedResponse }
            };

            nancyContext.SetMockInteraction(interactions);

            mockRequestMapper.Convert(nancyContext.Request).Returns(actualRequest);

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(mockRequestComparer, mockRequestMapper, mockResponseMapper);

            handler.Handle(nancyContext);

            mockRequestComparer.Received(1).Compare(expectedRequest, actualRequest);
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

            var mockRequestComparer = Substitute.For<IProviderServiceRequestComparer>();
            var mockRequestMapper = Substitute.For<IProviderServiceRequestMapper>();
            var mockResponseMapper = Substitute.For<INancyResponseMapper>();

            mockRequestMapper.Convert(nancyContext.Request).Returns(expectedRequest);

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction { Request = expectedRequest, Response = expectedResponse}
            };

            nancyContext.SetMockInteraction(interactions);

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(mockRequestComparer, mockRequestMapper, mockResponseMapper);

            handler.Handle(nancyContext);

            mockResponseMapper.Received(1).Convert(expectedResponse);
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

            var mockRequestComparer = Substitute.For<IProviderServiceRequestComparer>();
            var mockRequestMapper = Substitute.For<IProviderServiceRequestMapper>();
            var mockResponseMapper = Substitute.For<INancyResponseMapper>();

            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction { Request = expectedRequest, Response = expectedResponse }
            };

            nancyContext.SetMockInteraction(interactions);

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
            var expectedRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/Test"
            };
            var actualRequest = new ProviderServiceRequest
            {
                Method = HttpVerb.Put,
                Path = "/Test"
            };
            var expectedResponse = new ProviderServiceResponse { Status = 200 };
            var nancyResponse = new Response { StatusCode = HttpStatusCode.OK };
            var compareException = new CompareFailedException("Something failed");

            var mockRequestComparer = Substitute.For<IProviderServiceRequestComparer>();
            var mockRequestMapper = Substitute.For<IProviderServiceRequestMapper>();
            var mockResponseMapper = Substitute.For<INancyResponseMapper>();

            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction { Request = expectedRequest, Response = expectedResponse }
            };

            nancyContext.SetMockInteraction(interactions);

            mockRequestMapper.Convert(nancyContext.Request).Returns(actualRequest);
            mockRequestComparer
                .When(x => x.Compare(expectedRequest, actualRequest))
                .Do(x => { throw compareException; });

            mockResponseMapper.Convert(expectedResponse).Returns(nancyResponse);

            mockResponseMapper.Convert(Arg.Any<ProviderServiceResponse>())
                .Returns(new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(mockRequestComparer, mockRequestMapper, mockResponseMapper);

            var response = handler.Handle(nancyContext);

            mockResponseMapper.Received(1).Convert(Arg.Is<ProviderServiceResponse>(x => x.Status == 500));
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.NotEmpty(response.ReasonPhrase);
        }

        [Fact]
        public void Handle_WhenExpectionIsThrownHandlingRequest_ReasonPhraseAndBodyContentIsSetWithoutBackSlashes()
        {
            var nancyResponse = new Response { StatusCode = HttpStatusCode.OK };
            var compareException = new CompareFailedException("Something\r\n \t \\ failed");
            const string expectedMessage = "Something     failed";

            var mockRequestComparer = Substitute.For<IProviderServiceRequestComparer>();
            var mockRequestMapper = Substitute.For<IProviderServiceRequestMapper>();
            var mockResponseMapper = Substitute.For<INancyResponseMapper>();

            var nancyContext = new NancyContext
            {
                Request = new Request("GET", "/Test", "HTTP")
            };

            mockRequestMapper
                .When(x => x.Convert(Arg.Any<Request>()))
                .Do(x => { throw compareException; });

            mockResponseMapper
                .Convert(Arg.Any<ProviderServiceResponse>())
                .Returns(nancyResponse);

            mockResponseMapper.Convert(Arg.Any<ProviderServiceResponse>())
                .Returns(new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            IMockProviderNancyRequestHandler handler = new MockProviderNancyRequestHandler(mockRequestComparer, mockRequestMapper, mockResponseMapper);

            var response = handler.Handle(nancyContext);

            Assert.Equal(expectedMessage, response.ReasonPhrase);
            mockResponseMapper.Received(1).Convert(Arg.Is<ProviderServiceResponse>(x => BodyContentMatches(x, expectedMessage)));
        }

        private bool BodyContentMatches(ProviderServiceResponse response, string expectedBody)
        {
            if (response.Body == expectedBody)
            {
                return true;
            }

            return false;
        }
    }
}
