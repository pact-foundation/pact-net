using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using NSubstitute;
using Nancy;
using Nancy.IO;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Nancy;
using PactNet.Models;
using PactNet.Reporters;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Nancy
{
    public class MockProviderAdminRequestHandlerTests
    {
        private IMockProviderRepository _mockProviderRepository;
        private IReporter _mockReporter;
        private IFileSystem _mockFileSystem;

        private IMockProviderAdminRequestHandler GetSubject()
        {
            _mockProviderRepository = Substitute.For<IMockProviderRepository>();
            _mockReporter = Substitute.For<IReporter>();
            _mockFileSystem = Substitute.For<IFileSystem>();

            return new MockProviderAdminRequestHandler(
                _mockProviderRepository,
                _mockReporter,
                _mockFileSystem,
                new PactFileInfo(null));
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
        public void Handle_WithADeleteRequestToInteractions_ClearTestScopedInteractionsIsCalledOnTheMockProviderRepository()
        {
            var context = new NancyContext
            {
                Request = new Request("DELETE", "/interactions", "http")
            };

            var handler = GetSubject();

            handler.Handle(context);

            _mockProviderRepository.Received(1).ClearTestScopedInteractions();
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
        public void Handle_WithAPostRequestToInteractions_AddInteractionIsCalledOnTheMockProviderRepository()
        {
            var interaction = new ProviderServiceInteraction
            {
                Description = "My description",
                Request = new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/test"
                },
                Response = new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.NoContent
                }
            };
            var interactionJson = interaction.AsJsonString();

            var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(interactionJson));

            var requestStream = new RequestStream(jsonStream, jsonStream.Length, true);
            var context = new NancyContext
            {
                Request = new Request("POST", new Url("http://localhost/interactions"), requestStream)
            };

            var handler = GetSubject();

            handler.Handle(context);

            _mockProviderRepository.Received(1).AddInteraction(Arg.Is<ProviderServiceInteraction>(x => x.AsJsonString() == interactionJson));
        }

        [Fact]
        public void Handle_WithAPostRequestToInteractions_ReturnsOkResponse()
        {
            var interaction = new ProviderServiceInteraction
            {
                Description = "My description",
                Request = new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/test"
                },
                Response = new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.NoContent
                }
            };
            var interactionJson = interaction.AsJsonString();

            var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(interactionJson));

            var requestStream = new RequestStream(jsonStream, jsonStream.Length, true);
            var context = new NancyContext
            {
                Request = new Request("POST", new Url("http://localhost/interactions"), requestStream)
            };

            var handler = GetSubject();

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithALowercasedPostRequestToInteractions_ReturnsOkResponse()
        {
            var interaction = new ProviderServiceInteraction
            {
                Description = "My description",
                Request = new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/test"
                },
                Response = new ProviderServiceResponse
                {
                    Status = (int)HttpStatusCode.NoContent
                }
            };
            var interactionJson = interaction.AsJsonString();

            var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(interactionJson));

            var requestStream = new RequestStream(jsonStream, jsonStream.Length, true);
            var context = new NancyContext
            {
                Request = new Request("post", new Url("http://localhost/interactions"), requestStream)
            };

            var handler = GetSubject();

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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

            _mockProviderRepository.TestScopedInteractions.Returns(interactions);

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

            _mockProviderRepository.TestScopedInteractions.Returns(interactions);

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


            _mockProviderRepository.TestScopedInteractions.Returns(new List<ProviderServiceInteraction> { new ProviderServiceInteraction() });

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

            _mockProviderRepository.TestScopedInteractions.Returns(interactions);

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
                .Do(x => { throw new PactFailureException("Expected request cannot be null"); });

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

            _mockProviderRepository.TestScopedInteractions.Returns(new List<ProviderServiceInteraction> { new ProviderServiceInteraction() });

            _mockReporter
                .When(x => x.ThrowIfAnyErrors())
                .Do(x => { throw new PactFailureException(exceptionMessage); });

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

            _mockProviderRepository.TestScopedInteractions.Returns(new List<ProviderServiceInteraction> { new ProviderServiceInteraction() });

            _mockReporter
                .When(x => x.ThrowIfAnyErrors())
                .Do(x => { throw new PactFailureException(exceptionMessage); });

            var response = handler.Handle(context);

            var content = ReadResponseContent(response);

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
                .Do(x => { throw new PactFailureException("Expected request cannot be null"); });

            handler.Handle(context);

            _mockReporter.Received(1).ClearErrors();
        }

        [Fact]
        public void Handle_WithAPostRequestToPactAndNoInteractionsHaveBeenRegistered_NewPactFileIsSavedWithNoInteractions()
        {
            var pactDetails = new PactDetails
            {
                Consumer = new Party { Name = "Consumer" },
                Provider = new Party { Name = "Provider" }
            };

            var pactFile = new ProviderServicePactFile
            {
                Provider = pactDetails.Provider,
                Consumer = pactDetails.Consumer,
                Interactions = new ProviderServiceInteraction[0]
            };

            var pactFileJson = JsonConvert.SerializeObject(pactFile, JsonConfig.PactFileSerializerSettings);
            var pactDetailsJson = JsonConvert.SerializeObject(pactDetails, JsonConfig.ApiSerializerSettings);

            var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(pactDetailsJson));

            var requestStream = new RequestStream(jsonStream, jsonStream.Length, true);
            var context = new NancyContext
            {
                Request = new Request("POST", new Url("http://localhost/pact"), requestStream)
            };

            var handler = GetSubject();

            handler.Handle(context);

            _mockFileSystem.File.Received(1).WriteAllText(Path.Combine(Constants.DefaultPactFileDirectory, pactDetails.GeneratePactFileName()), pactFileJson);
        }

        [Fact]
        public void Handle_WithAPostRequestToPactAndInteractionsHaveBeenRegistered_NewPactFileIsSavedWithInteractions()
        {
            var pactDetails = new PactDetails
            {
                Consumer = new Party { Name = "Consumer" },
                Provider = new Party { Name = "Provider" }
            };

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction
                {
                    Description = "My description",
                    Request = new ProviderServiceRequest
                    {
                        Method = HttpVerb.Get,
                        Path = "/test"
                    },
                    Response = new ProviderServiceResponse
                    {
                        Status = (int)HttpStatusCode.NoContent
                    }
                }
            };

            var pactFile = new ProviderServicePactFile
            {
                Provider = pactDetails.Provider,
                Consumer = pactDetails.Consumer,
                Interactions = interactions
            };

            var pactFileJson = JsonConvert.SerializeObject(pactFile, JsonConfig.PactFileSerializerSettings);
            var pactDetailsJson = JsonConvert.SerializeObject(pactDetails, JsonConfig.ApiSerializerSettings);

            var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(pactDetailsJson));

            var requestStream = new RequestStream(jsonStream, jsonStream.Length, true);
            var context = new NancyContext
            {
                Request = new Request("POST", new Url("http://localhost/pact"), requestStream)
            };

            var handler = GetSubject();

            _mockProviderRepository.Interactions.Returns(interactions);

            handler.Handle(context);

            _mockFileSystem.File.Received(1).WriteAllText(Path.Combine(Constants.DefaultPactFileDirectory, pactDetails.GeneratePactFileName()), pactFileJson);
        }

        [Fact]
        public void Handle_WithAPostRequestToPactAndInteractionsHaveBeenRegistered_ReturnsOkResponse()
        {
            var pactDetails = new PactDetails
            {
                Consumer = new Party { Name = "Consumer" },
                Provider = new Party { Name = "Provider" }
            };

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction
                {
                    Description = "My description",
                    Request = new ProviderServiceRequest
                    {
                        Method = HttpVerb.Get,
                        Path = "/test"
                    },
                    Response = new ProviderServiceResponse
                    {
                        Status = (int)HttpStatusCode.NoContent
                    }
                }
            };

            var pactDetailsJson = JsonConvert.SerializeObject(pactDetails, JsonConfig.ApiSerializerSettings);

            var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(pactDetailsJson));

            var requestStream = new RequestStream(jsonStream, jsonStream.Length, true);
            var context = new NancyContext
            {
                Request = new Request("POST", new Url("http://localhost/pact"), requestStream)
            };

            var handler = GetSubject();

            _mockProviderRepository.Interactions.Returns(interactions);

            var response = handler.Handle(context);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithAPostRequestToPactAndInteractionsHaveBeenRegistered_ReturnsResponseWithPactFileJson()
        {
            var pactDetails = new PactDetails
            {
                Consumer = new Party { Name = "Consumer" },
                Provider = new Party { Name = "Provider" }
            };

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction
                {
                    Description = "My description",
                    Request = new ProviderServiceRequest
                    {
                        Method = HttpVerb.Get,
                        Path = "/test"
                    },
                    Response = new ProviderServiceResponse
                    {
                        Status = (int)HttpStatusCode.NoContent
                    }
                }
            };

            var pactFile = new ProviderServicePactFile
            {
                Provider = pactDetails.Provider,
                Consumer = pactDetails.Consumer,
                Interactions = interactions
            };

            var pactFileJson = JsonConvert.SerializeObject(pactFile, JsonConfig.PactFileSerializerSettings);
            var pactDetailsJson = JsonConvert.SerializeObject(pactDetails, JsonConfig.ApiSerializerSettings);

            var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(pactDetailsJson));

            var requestStream = new RequestStream(jsonStream, jsonStream.Length, true);
            var context = new NancyContext
            {
                Request = new Request("POST", new Url("http://localhost/pact"), requestStream)
            };

            var handler = GetSubject();

            _mockProviderRepository.Interactions.Returns(interactions);

            var response = handler.Handle(context);

            Assert.Equal("application/json", response.Headers["Content-Type"]);
            Assert.Equal(pactFileJson, ReadResponseContent(response.Contents));
        }

        [Fact]
        public void Handle_WithAPostRequestToPactAndDirectoryDoesNotExist_DirectoryIsCreatedAndNewPactFileIsSavedWithInteractions()
        {
            var pactDetails = new PactDetails
            {
                Consumer = new Party { Name = "Consumer" },
                Provider = new Party { Name = "Provider" }
            };

            var interactions = new List<ProviderServiceInteraction>
            {
                new ProviderServiceInteraction
                {
                    Description = "My description",
                    Request = new ProviderServiceRequest
                    {
                        Method = HttpVerb.Get,
                        Path = "/test"
                    },
                    Response = new ProviderServiceResponse
                    {
                        Status = (int)HttpStatusCode.NoContent
                    }
                }
            };

            var pactFile = new ProviderServicePactFile
            {
                Provider = pactDetails.Provider,
                Consumer = pactDetails.Consumer,
                Interactions = interactions
            };

            var pactFileJson = JsonConvert.SerializeObject(pactFile, JsonConfig.PactFileSerializerSettings);
            var pactDetailsJson = JsonConvert.SerializeObject(pactDetails, JsonConfig.ApiSerializerSettings);

            var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(pactDetailsJson));

            var requestStream = new RequestStream(jsonStream, jsonStream.Length, true);
            var context = new NancyContext
            {
                Request = new Request("POST", new Url("http://localhost/pact"), requestStream)
            };

            var filePath = Path.Combine(Constants.DefaultPactFileDirectory, pactDetails.GeneratePactFileName());

            var handler = GetSubject();

            _mockProviderRepository.Interactions.Returns(interactions);

            var writeAllTextCount = 0;
            _mockFileSystem.File
                .When(x => x.WriteAllText(filePath, pactFileJson))
                .Do(x =>
                {
                    writeAllTextCount++;
                    if (writeAllTextCount == 1)
                    {
                        throw new DirectoryNotFoundException("It doesn't exist");
                    }
                });

            handler.Handle(context);

            _mockFileSystem.File.Received(2).WriteAllText(filePath, pactFileJson);
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
