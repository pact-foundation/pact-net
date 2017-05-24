using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NSubstitute;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Infrastructure;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Nancy
{
    public class MockProviderAdminRequestHandlerTests
    {
        private IMockProviderRepository _mockProviderRepository;
        private IFileSystem _mockFileSystem;
        private ILog _mockLog;
        
        private IMockProviderAdminRequestHandler GetSubject(PactConfig pactConfig = null)
        {
            _mockProviderRepository = Substitute.For<IMockProviderRepository>();
            _mockFileSystem = Substitute.For<IFileSystem>();
            _mockLog = Substitute.For<ILog>();

            _mockLog.Log(Arg.Any<LogLevel>(), Arg.Any<Func<string>>(), Arg.Any<Exception>(), Arg.Any<object[]>())
                .Returns(true);

            return new MockProviderAdminRequestHandler(
                _mockProviderRepository,
                _mockFileSystem,
                pactConfig ?? new PactConfig(),
                _mockLog);
        }

        [Fact]
        public void Handle_WithTheTestContextHeaderAttached_LogsTheTestContext()
        {
            const string testContext = "EventsApiConsumerTests.GetAllEvents_WhenCalled_ReturnsAllEvents";
            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { Constants.AdministrativeRequestTestContextHeaderKey, new List<string> { testContext } }
            };
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("DELETE");
            request.Path.Returns("/interactions");
            request.Headers.Returns(headers);

            var handler = GetSubject();

            handler.Handle(request);

            _mockLog.Received(1).Log(LogLevel.Info, Arg.Any<Func<string>>(), null, Arg.Is<object[]>(x => x.Single().ToString() == testContext));
        }

        [Fact]
        public void Handle_WithTheTestContextHeaderAttached_SetsTestContextOnTheRepository()
        {
            const string testContext = "EventsApiConsumerTests.GetAllEvents_WhenCalled_ReturnsAllEvents";
            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { Constants.AdministrativeRequestTestContextHeaderKey, new List<string> { testContext } }
            };
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("DELETE");
            request.Path.Returns("/interactions");
            request.Headers.Returns(headers);

            var handler = GetSubject();

            handler.Handle(request);

            _mockProviderRepository.Received(1).TestContext = testContext;
        }

        [Fact]
        public void Handle_WhenTestContextIsSetOnTheRepository_DoesNotLogTheTextContext()
        {
            const string testContext = "EventsApiConsumerTests.GetAllEvents_WhenCalled_ReturnsAllEvents";
            var headers = new Dictionary<string, IEnumerable<string>>
            {
                { Constants.AdministrativeRequestTestContextHeaderKey, new List<string> { testContext } }
            };
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("DELETE");
            request.Path.Returns("/interactions");
            request.Headers.Returns(headers);

            var handler = GetSubject();

            _mockProviderRepository.TestContext.Returns(testContext);

            handler.Handle(request);

            _mockLog.Received(0).Log(LogLevel.Info, Arg.Any<Func<string>>(), null, Arg.Is<object[]>(x => x.Single().ToString() == testContext));
        }

        [Fact]
        public void Handle_WithADeleteRequestToInteractions_ClearHandledRequestsIsCalledOnTheMockProviderRepository()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("DELETE");
            request.Path.Returns("/interactions");
            
            var handler = GetSubject();

            handler.Handle(request);

            _mockProviderRepository.Received(1).ClearTestScopedState();
        }

        [Fact]
        public void Handle_WithADeleteRequestToInteractions_ClearTestScopedInteractionsIsCalledOnTheMockProviderRepository()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("DELETE");
            request.Path.Returns("/interactions");

            var handler = GetSubject();

            handler.Handle(request);

            _mockProviderRepository.Received(1).ClearTestScopedState();
        }

        [Fact]
        public void Handle_WithADeleteRequestToInteractions_ReturnsOkResponse()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("DELETE");
            request.Path.Returns("/interactions");

            var handler = GetSubject();

            var response = handler.Handle(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithALowercasedDeleteRequestToInteractions_ReturnsOkResponse()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("DELETE");
            request.Path.Returns("/interactions");
            
            var handler = GetSubject();

            var response = handler.Handle(request);

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
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("POST");
            request.Path.Returns("/interactions");
            request.Body.Returns(Encoding.UTF8.GetBytes(interactionJson));

            var handler = GetSubject();

            handler.Handle(request);

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
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("POST");
            request.Path.Returns("/interactions");
            request.Body.Returns(Encoding.UTF8.GetBytes(interactionJson));

            var handler = GetSubject();

            var response = handler.Handle(request);

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
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("post");
            request.Path.Returns("/interactions");
            request.Body.Returns(Encoding.UTF8.GetBytes(interactionJson));

            var handler = GetSubject();

            var response = handler.Handle(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerification_ReturnsOkResponse()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/interactions/verification");

            var handler = GetSubject();

            var response = handler.Handle(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithALowercasedGetRequestToInteractionsVerification_ReturnsOkResponse()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/interactions/verification");

            var handler = GetSubject();

            var response = handler.Handle(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndRegisteredInteractionWasCalledExactlyOnce_ReturnsOkResponse()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/interactions/verification");

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

            var response = handler.Handle(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndRegisteredInteractionWasNotCalled_ThrowsPactFailureExceptionAndLogsTheMissingRequest()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/interactions/verification");

            var handler = GetSubject();


            _mockProviderRepository.TestScopedInteractions.Returns(new List<ProviderServiceInteraction> { new ProviderServiceInteraction() });

            Assert.Throws<PactFailureException>(() => handler.Handle(request));

            _mockLog.Received().Log(LogLevel.Error, Arg.Any<Func<string>>(), null, Arg.Any<object[]>());
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndRegisteredInteractionWasCalledMultipleTimes_ThrowsPactFailureExceptionAndLogsTheError()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/interactions/verification");

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

            Assert.Throws<PactFailureException>(() => handler.Handle(request));

            _mockLog.Received().Log(LogLevel.Error, Arg.Any<Func<string>>(), null, Arg.Any<object[]>());
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndNoInteractionsRegisteredHoweverMockProviderRecievedInteractions_ThrowsPactFailureException()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/interactions/verification");

            var handler = GetSubject();

            _mockProviderRepository.HandledRequests.Returns(new List<HandledRequest>
            {
                new HandledRequest(new ProviderServiceRequest(), new ProviderServiceInteraction())
            });

            Assert.Throws<PactFailureException>(() => handler.Handle(request));
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndCorrectlyMatchedHandledRequest_ReturnsOkResponse()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/interactions/verification");

            var interaction = new ProviderServiceInteraction();

            var handler = GetSubject();
            

            _mockProviderRepository.TestScopedInteractions.Returns(new List<ProviderServiceInteraction>
            {
                interaction
            });

            _mockProviderRepository.HandledRequests.Returns(new List<HandledRequest>
            {
                new HandledRequest(new ProviderServiceRequest(), interaction)
            });

            var response = handler.Handle(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndAnIncorrectlyMatchedHandledRequest_ThrowsPactFailureException()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/interactions/verification");

            var handler = GetSubject();

            _mockProviderRepository.HandledRequests.Returns(new List<HandledRequest>
            {
                new HandledRequest(new ProviderServiceRequest(), new ProviderServiceInteraction())
            });

            Assert.Throws<PactFailureException>(() => handler.Handle(request));
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndAnInteractionWasSentButNotRegisteredByTheTest_ThrowsPactFailureExceptionWithTheCorrectMessageAndLogsTheUnexpectedRequest()
        {
            const string failure = "An unexpected request POST /tester was seen by the mock provider service.";
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/interactions/verification");

            var handler = GetSubject();

            var handledRequest = new ProviderServiceRequest();
            var handledInteraction = new ProviderServiceInteraction { Request = handledRequest };

            var unExpectedRequest = new ProviderServiceRequest { Method = HttpVerb.Post, Path = "/tester" };

            _mockProviderRepository.TestScopedInteractions
                .Returns(new List<ProviderServiceInteraction>
                {
                    handledInteraction
                });

            _mockProviderRepository.HandledRequests
                .Returns(new List<HandledRequest>
                {
                    new HandledRequest(handledRequest, handledInteraction),
                    new HandledRequest(unExpectedRequest, null)
                });

            var exception = Assert.Throws<PactFailureException>(() => handler.Handle(request));

            _mockLog.Received().Log(LogLevel.Error, Arg.Any<Func<string>>(), null, Arg.Any<object[]>());
            Assert.Equal(failure, exception.Message);
        }

        [Fact]
        public void Handle_WithAGetRequestToInteractionsVerificationAndAFailureOcurrs_ThrowsPactFailureExceptionWithTheCorrectMessage()
        {
            const string failure = "The interaction with description '' and provider state '', was not used by the test. Missing request No Method No Path.";
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/interactions/verification");

            var handler = GetSubject();

            _mockProviderRepository.TestScopedInteractions.Returns(new List<ProviderServiceInteraction> { new ProviderServiceInteraction() });

            var expection = Assert.Throws<PactFailureException>(() => handler.Handle(request));

            Assert.Equal(failure, expection.Message);
        }

        [Fact]
        public void Handle_WithAPostRequestToPactAndNoInteractionsHaveBeenRegistered_NewPactFileIsSavedWithNoInteractions()
        {
            var pactDetails = new PactDetails
            {
                Consumer = new Pacticipant { Name = "Consumer" },
                Provider = new Pacticipant { Name = "Provider" }
            };

            var pactFile = new ProviderServicePactFile
            {
                Provider = pactDetails.Provider,
                Consumer = pactDetails.Consumer,
                Interactions = new ProviderServiceInteraction[0]
            };

            var pactFileJson = JsonConvert.SerializeObject(pactFile, JsonConfig.PactFileSerializerSettings);
            var pactDetailsJson = JsonConvert.SerializeObject(pactDetails, JsonConfig.ApiSerializerSettings);
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("POST");
            request.Path.Returns("/pact");
            request.Body.Returns(Encoding.UTF8.GetBytes(pactDetailsJson));

            var handler = GetSubject();

            handler.Handle(request);

            _mockFileSystem.File.Received(1).WriteAllText(Path.Combine(Constants.DefaultPactDir, pactDetails.GeneratePactFileName()), pactFileJson);
        }

        [Fact]
        public void Handle_WithAPostRequestToPactAndInteractionsHaveBeenRegistered_NewPactFileIsSavedWithInteractions()
        {
            var pactDetails = new PactDetails
            {
                Consumer = new Pacticipant { Name = "Consumer" },
                Provider = new Pacticipant { Name = "Provider" }
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
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("POST");
            request.Path.Returns("/pact");
            request.Body.Returns(Encoding.UTF8.GetBytes(pactDetailsJson));
            
            var handler = GetSubject();

            _mockProviderRepository.Interactions.Returns(interactions);

            handler.Handle(request);

            _mockFileSystem.File.Received(1).WriteAllText(Path.Combine(Constants.DefaultPactDir, pactDetails.GeneratePactFileName()), pactFileJson);
        }

        [Fact]
        public void Handle_WithAPostRequestToPactAndInteractionsHaveBeenRegistered_ReturnsOkResponse()
        {
            var pactDetails = new PactDetails
            {
                Consumer = new Pacticipant { Name = "Consumer" },
                Provider = new Pacticipant { Name = "Provider" }
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
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("POST");
            request.Path.Returns("/pact");
            request.Body.Returns(Encoding.UTF8.GetBytes(pactDetailsJson));
            
            var handler = GetSubject();

            _mockProviderRepository.Interactions.Returns(interactions);

            var response = handler.Handle(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Handle_WithAPostRequestToPactAndInteractionsHaveBeenRegistered_ReturnsResponseWithPactFileJson()
        {
            var pactDetails = new PactDetails
            {
                Consumer = new Pacticipant { Name = "Consumer" },
                Provider = new Pacticipant { Name = "Provider" }
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
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("POST");
            request.Path.Returns("/pact");
            request.Body.Returns(Encoding.UTF8.GetBytes(pactDetailsJson));

            var handler = GetSubject();

            _mockProviderRepository.Interactions.Returns(interactions);

            var response = handler.Handle(request);

            Assert.Equal("application/json", response.Headers["Content-Type"]);
            Assert.Equal(pactFileJson, Encoding.UTF8.GetString(response.Contents));
        }

        [Fact]
        public void Handle_WithAPostRequestToPactAndPactDirIsDifferentFromDefault_NewPactFileIsSavedWithInteractionsInTheSpecifiedPath()
        {
            var pactDetails = new PactDetails
            {
                Consumer = new Pacticipant { Name = "Consumer" },
                Provider = new Pacticipant { Name = "Provider" }
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

            var config = new PactConfig { PactDir = @"C:\temp" };
            var filePath = Path.Combine(config.PactDir, pactDetails.GeneratePactFileName());

            var pactFileJson = JsonConvert.SerializeObject(pactFile, JsonConfig.PactFileSerializerSettings);
            var pactDetailsJson = JsonConvert.SerializeObject(pactDetails, JsonConfig.ApiSerializerSettings);
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("POST");
            request.Path.Returns("/pact");
            request.Body.Returns(Encoding.UTF8.GetBytes(pactDetailsJson));
            
            var handler = GetSubject(config);

            _mockProviderRepository.Interactions.Returns(interactions);

            var response = handler.Handle(request);

            _mockFileSystem.File.Received(1).WriteAllText(filePath, pactFileJson);
        }

        [Fact]
        public void Handle_WithAPostRequestToPactAndDirectoryDoesNotExist_DirectoryIsCreatedAndNewPactFileIsSavedWithInteractions()
        {
            var pactDetails = new PactDetails
            {
                Consumer = new Pacticipant { Name = "Consumer" },
                Provider = new Pacticipant { Name = "Provider" }
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
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("POST");
            request.Path.Returns("/pact");
            request.Body.Returns(Encoding.UTF8.GetBytes(pactDetailsJson));
            
            var filePath = Path.Combine(Constants.DefaultPactDir, pactDetails.GeneratePactFileName());

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

            handler.Handle(request);

            _mockFileSystem.File.Received(2).WriteAllText(filePath, pactFileJson);
        }

        [Fact]
        public void Handle_WhenNoMatchingAdminAction_ReturnsNotFoundResponse()
        {
            var request = Substitute.For<IRequestWrapper>();

            request.Method.Returns("GET");
            request.Path.Returns("/tester/testing");

            var handler = GetSubject();

            var response = handler.Handle(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
