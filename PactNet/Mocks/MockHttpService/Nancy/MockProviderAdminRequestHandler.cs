using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Nancy;
using Newtonsoft.Json;
using PactNet.Comparers;
using PactNet.Configuration.Json;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Models;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    internal class MockProviderAdminRequestHandler : IMockProviderAdminRequestHandler
    {
        private readonly IMockProviderRepository _mockProviderRepository;
        private readonly IFileSystem _fileSystem;
        private readonly string _pactFileDirectory;
        private readonly ILog _log;

        public MockProviderAdminRequestHandler(
            IMockProviderRepository mockProviderRepository,
            IFileSystem fileSystem,
            PactFileInfo pactFileInfo,
            ILog log)
        {
            _mockProviderRepository = mockProviderRepository;
            _fileSystem = fileSystem;
            _pactFileDirectory = pactFileInfo.Directory ?? Constants.DefaultPactFileDirectory;
            _log = log;
        }

        public Response Handle(NancyContext context)
        {
            if (context.Request.Method.Equals("DELETE", StringComparison.InvariantCultureIgnoreCase) &&
                context.Request.Path == Constants.InteractionsPath)
            {
                return HandleDeleteInteractionsRequest();
            }

            if (context.Request.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase) &&
                context.Request.Path == Constants.InteractionsPath)
            {
                return HandlePostInteractionsRequest(context);
            }

            if (context.Request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase) &&
                context.Request.Path == Constants.InteractionsVerificationPath)
            {
                return HandleGetInteractionsVerificationRequest();
            }

            if (context.Request.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase) &&
                context.Request.Path == Constants.PactPath)
            {
                return HandlePostPactRequest(context);
            }

            return GenerateResponse(HttpStatusCode.NotFound,
                String.Format("The {0} request for path {1}, does not have a matching mock provider admin action.", context.Request.Method, context.Request.Path));
        }

        private Response HandleDeleteInteractionsRequest()
        {
            _mockProviderRepository.ClearHandledRequests();
            _mockProviderRepository.ClearTestScopedInteractions();

            _log.Info("Cleared interactions");
            
            return GenerateResponse(HttpStatusCode.OK, "Deleted interactions");
        }

        private Response HandlePostInteractionsRequest(NancyContext context)
        {
            var interactionJson = ReadContent(context.Request.Body);
            var interaction = JsonConvert.DeserializeObject<ProviderServiceInteraction>(interactionJson);
            _mockProviderRepository.AddInteraction(interaction);

            _log.InfoFormat("Registered expected interaction {0} {1}", interaction.Request.Method.ToString().ToUpperInvariant(), interaction.Request.Path);
            _log.Debug(JsonConvert.SerializeObject(interaction, Formatting.Indented));

            return GenerateResponse(HttpStatusCode.OK, "Added interaction");
        }

        private Response HandleGetInteractionsVerificationRequest()
        {
            //TODO: add logs for this! Trello has the example
            //Verifying - actual interactions do not match expected interactions for example "ZooApp::AnimalServiceClient.find_alligator_by_name when an alligator by the given name does not exist returns nil". 
//Missing requests:
//	GET /alligators/turds

//Unexpected requests:
//	GET /alligators/tester


            //Check all registered interactions have been used once and only once
            var registeredInteractions = _mockProviderRepository.TestScopedInteractions;

            var comparisonResult = new ComparisonResult();

            if (registeredInteractions.Any())
            {
                foreach (var registeredInteraction in registeredInteractions)
                {
                    var interactionUsages = _mockProviderRepository.HandledRequests.Where(x => x.MatchedInteraction != null && x.MatchedInteraction == registeredInteraction).ToList();

                    if (interactionUsages == null || !interactionUsages.Any())
                    {
                        var missingRequestMethod = registeredInteraction.Request != null ? registeredInteraction.Request.Method.ToString().ToUpperInvariant() : "No Method";
                        var missingRequestPath = registeredInteraction.Request != null ? registeredInteraction.Request.Path : "No Path";

                        comparisonResult.RecordFailure(String.Format("The interaction with description '{0}' and provider state '{1}', was not used by the test. Missing request {2} {3}.", 
                            registeredInteraction.Description, 
                            registeredInteraction.ProviderState,
                            missingRequestMethod, 
                            missingRequestPath));

                        _log.Warn("Verifying - actual interactions do not match expected interactions");
                        _log.WarnFormat("Missing request: {0} {1}", missingRequestMethod, missingRequestPath); //TODO: Make a collection and write at bottom
                    }
                    else if (interactionUsages.Count() > 1)
                    {
                        comparisonResult.RecordFailure(String.Format("The interaction with description '{0}' and provider state '{1}', was used {2} time/s by the test.", registeredInteraction.Description, registeredInteraction.ProviderState, interactionUsages.Count()));

                        _log.Warn("Verifying - actual interactions do not match expected interactions");
                        //TODO: More info here
                    }
                }
            }
            else
            {
                if (_mockProviderRepository.HandledRequests != null && _mockProviderRepository.HandledRequests.Any())
                {
                    comparisonResult.RecordFailure("No interactions were registered, however the mock provider service was called.");

                    _log.Warn("Verifying - actual interactions do not match expected interactions");
                    //TODO: More info here
                }
            }

            //TODO: Test this functionality
            if (_mockProviderRepository.HandledRequests != null && _mockProviderRepository.HandledRequests.Any(x => x.MatchedInteraction == null))
            {
                foreach (var handledRequest in _mockProviderRepository.HandledRequests.Where(x => x.MatchedInteraction == null))
                {
                    var unexpectedRequestMethod = handledRequest.ActualRequest != null ? handledRequest.ActualRequest.Method.ToString().ToUpperInvariant() : "No Method";
                    var unexpectedRequestPath = handledRequest.ActualRequest != null ? handledRequest.ActualRequest.Path : "No Path";

                    comparisonResult.RecordFailure(String.Format("An unexpected request {0} {1} was seen by the mock provider service.",
                            unexpectedRequestMethod,
                            unexpectedRequestPath));

                    _log.Warn("Verifying - actual interactions do not match expected interactions"); //TODO: Should only print this once for a failure
                    _log.WarnFormat("Unexpected request: {0} {1}", unexpectedRequestMethod, unexpectedRequestPath); //TODO: Make a collection and write at bottom
                }
            }
            
            if (!comparisonResult.HasFailure)
            {
                _log.Info("Verifying - interactions matched");

                return GenerateResponse(HttpStatusCode.OK, "Interactions matched");
            }
            
            var failure = comparisonResult.Failures.First();
            return GenerateResponse(HttpStatusCode.InternalServerError, failure.Result);
        }

        private Response HandlePostPactRequest(NancyContext context)
        {
            var pactDetailsJson = ReadContent(context.Request.Body);
            var pactDetails = JsonConvert.DeserializeObject<PactDetails>(pactDetailsJson);
            var pactFilePath = Path.Combine(_pactFileDirectory, pactDetails.GeneratePactFileName());

            var pactFile = new ProviderServicePactFile
            {
                Provider = pactDetails.Provider,
                Consumer = pactDetails.Consumer,
                Interactions = _mockProviderRepository.Interactions
            };

            var pactFileJson = JsonConvert.SerializeObject(pactFile, JsonConfig.PactFileSerializerSettings);

            try
            {
                _fileSystem.File.WriteAllText(pactFilePath, pactFileJson);
            }
            catch (DirectoryNotFoundException)
            {
                _fileSystem.Directory.CreateDirectory(_pactFileDirectory);
                _fileSystem.File.WriteAllText(pactFilePath, pactFileJson);
            }

            return GenerateResponse(HttpStatusCode.OK, pactFileJson, "application/json");
        }

        private Response GenerateResponse(HttpStatusCode statusCode, string message, string contentType = "text/plain")
        {
            return new Response
            {
                StatusCode = statusCode,
                Headers = new Dictionary<string, string> { { "Content-Type", contentType } },
                Contents = s => SetContent(message, s)
            };
        }

        private void SetContent(string content, Stream stream)
        {
            var contentBytes = Encoding.UTF8.GetBytes(content);
            stream.Write(contentBytes, 0, contentBytes.Length);
            stream.Flush();
        }

        private string ReadContent(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}