using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSubstitute;
using Nancy;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Nancy;
using PactNet.Reporters;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Nancy
{
    public class MockProviderAdminRequestHandlerTests
    {
        private IMockProviderRepository _mockProviderRepository;
        private IReporter _mockReporter;
        private IProviderServiceRequestComparer _mockRequestComparer;

        private IMockProviderAdminRequestHandler GetSubject()
        {
            _mockProviderRepository = Substitute.For<IMockProviderRepository>();
            _mockReporter = Substitute.For<IReporter>();
            _mockRequestComparer = Substitute.For<IProviderServiceRequestComparer>();

            return new MockProviderAdminRequestHandler(
                _mockProviderRepository,
                _mockReporter,
                _mockRequestComparer);
        }

        [Fact]
        public void Handle_WithADeleteRequestToInteractions_ClearHandledRequestsIsCalledOnTheMockProviderRepository()
        {
            var context = new NancyContext
            {
                Request = new Request("DELETE", "/interactions", "http")
            };

            var handler = GetSubject();

            handler.Handle(context);

            _mockProviderRepository.Received(1).ClearHandledRequests();
        }

        [Fact]
        public void Handle_WithADeleteRequestToInteractions_ReturnsOkResponse()
        {
            var context = new NancyContext
            {
                Request = new Request("DELETE", "/interactions", "http")
            };

            var handler = GetSubject();

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithALowercasedDeleteRequestToInteractions_ReturnsOkResponse()
        {
            var context = new NancyContext
            {
                Request = new Request("delete", "/interactions", "http")
            };

            var handler = GetSubject();

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WhenNoMatchingAdminAction_ReturnsNotFoundResponse()
        {
            var context = new NancyContext
            {
                Request = new Request("GET", "/tester/testing", "http")
            };

            var handler = GetSubject();

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerification_ReportErrorIsNotCalledOnReporter()
        {
            var context = new NancyContext
            {
                Request = new Request("GET", "/interactions/verification", "http")
            };

            var handler = GetSubject();

            handler.Handle(context);

            _mockReporter.DidNotReceive().ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerification_ReturnsOkResponse()
        {
            var context = new NancyContext
            {
                Request = new Request("GET", "/interactions/verification", "http")
            };

            var handler = GetSubject();

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithALowercasedGetRequestToInteractionsVerification_ReturnsOkResponse()
        {
            var context = new NancyContext
            {
                Request = new Request("get", "/interactions/verification", "http")
            };

            var handler = GetSubject();

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndRegisteredInteractionWasCalledExactlyOnce_ReturnsOkResponse()
        {
            var context = new NancyContext
            {
                Request = new Request("GET", "/interactions/verification", "http")
            };

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction()
            };

            var handler = GetSubject();

            context.SetMockInteraction(interactions);

            _mockProviderRepository.HandledRequests.Returns(new List<HandledRequest>
            {
                new HandledRequest(new ProviderServiceRequest(), interactions.First())
            });

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndRegisteredInteractionWasCalledExactlyOnce_ReportErrorIsNotCalledOnReporter()
        {
            var context = new NancyContext
            {
                Request = new Request("GET", "/interactions/verification", "http")
            };

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction()
            };

            var handler = GetSubject();

            context.SetMockInteraction(interactions);

            _mockProviderRepository.HandledRequests.Returns(new List<HandledRequest>
            {
                new HandledRequest(new ProviderServiceRequest(), interactions.First())
            });

            handler.Handle(context);

            _mockReporter.DidNotReceive().ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndRegisteredInteractionWasNotCalled_ReportErrorIsCalledOnReporter()
        {
            var context = new NancyContext
            {
                Request = new Request("GET", "/interactions/verification", "http")
            };
            
            var handler = GetSubject();

            context.SetMockInteraction(new List<ProviderServiceInteraction> { new ProviderServiceInteraction() });

            handler.Handle(context);

            _mockReporter.Received(1).ReportError(Arg.Any<string>());
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndRegisteredInteractionWasCalledMultipleTimes_ReportErrorIsCalledOnReporter()
        {
            var context = new NancyContext
            {
                Request = new Request("GET", "/interactions/verification", "http")
            };

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction()
            };

            var handler = GetSubject();

            context.SetMockInteraction(interactions);

            _mockProviderRepository.HandledRequests.Returns(new List<HandledRequest>
            {
                new HandledRequest(new ProviderServiceRequest(), interactions.First()),
                new HandledRequest(new ProviderServiceRequest(), interactions.First())
            });

            handler.Handle(context);

            _mockReporter.Received(1).ReportError(Arg.Any<string>());
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndNoInteractionsRegisteredHoweverMockProviderRecievedInteractions_ReportErrorIsCalledOnReporter()
        {
            var context = new NancyContext
            {
                Request = new Request("GET", "/interactions/verification", "http")
            };

            var handler = GetSubject();

            _mockProviderRepository.HandledRequests.Returns(new List<HandledRequest>
            {
                new HandledRequest(new ProviderServiceRequest(), new ProviderServiceInteraction())
            });

            handler.Handle(context);

            _mockReporter.Received(1).ReportError(Arg.Any<string>());
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndCorrectlyMatchedHandledRequest_ReturnsOkResponse()
        {
            var context = new NancyContext
            {
                Request = new Request("GET", "/interactions/verification", "http")
            };

            var handler = GetSubject();

            _mockProviderRepository.HandledRequests.Returns(new List<HandledRequest>
            {
                new HandledRequest(new ProviderServiceRequest(), new ProviderServiceInteraction())
            });

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndAnIncorrectlyMatchedHandledRequest_ReturnsInternalServerErrorResponse()
        {
            var context = new NancyContext
            {
                Request = new Request("GET", "/interactions/verification", "http")
            };

            var handler = GetSubject();

            _mockProviderRepository.HandledRequests.Returns(new List<HandledRequest>
            {
                new HandledRequest(new ProviderServiceRequest(), new ProviderServiceInteraction())
            });

            _mockReporter
                .When(x => x.ThrowIfAnyErrors())
                .Do(x => { throw new CompareFailedException("Expected request cannot be null"); });

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndReporterHasErrors_ReturnsInternalServerErrorResponse()
        {
            const string exceptionMessage = "Registered mock interaction with description 'My description' and provider state 'My provider state', was not used by the test.";
            var context = new NancyContext
            {
                Request = new Request("GET", "/interactions/verification", "http")
            };

            var handler = GetSubject();
            
            context.SetMockInteraction(new List<ProviderServiceInteraction> { new ProviderServiceInteraction() });

            _mockReporter
                .When(x => x.ThrowIfAnyErrors())
                .Do(x => { throw new CompareFailedException(exceptionMessage); });

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndReporterHasErrors_ReturnsErrorResponseContent()
        {
            const string exceptionMessage = "Registered mock interaction with description 'My description' and provider state 'My provider state', was not used by the test.";
            var context = new NancyContext
            {
                Request = new Request("GET", "/interactions/verification", "http")
            };

            var handler = GetSubject();

            context.SetMockInteraction(new List<ProviderServiceInteraction> { new ProviderServiceInteraction() });

            _mockReporter
                .When(x => x.ThrowIfAnyErrors())
                .Do(x => { throw new CompareFailedException(exceptionMessage); });

            var response = handler.Handle(context);

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

            Assert.Equal(exceptionMessage, content);
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndReporterHasErrors_ClearErrorsIsCalledOnReporter()
        {
            var context = new NancyContext
            {
                Request = new Request("GET", "/interactions/verification", "http")
            };

            var handler = GetSubject();

            _mockReporter
                .When(x => x.ThrowIfAnyErrors())
                .Do(x => { throw new CompareFailedException("Expected request cannot be null"); });

            handler.Handle(context);

            _mockReporter.Received(1).ClearErrors();
        }
    }
}
