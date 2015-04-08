using System;
using Nancy;
using Newtonsoft.Json;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public enum LogType
    {
        Debug,
        Info,
        Error,
        Warn
    }

    public interface ILogger
    {
        void Log(string message, LogType type = LogType.Info);
    }

    public class Logger : ILogger
    {
        //need to write to /log/

        public void Log(string message, LogType type = LogType.Info)
        {
            
        }
    }

    internal class MockProviderRequestHandler : IMockProviderRequestHandler
    {
        private readonly INancyResponseMapper _responseMapper;
        private readonly IProviderServiceRequestMapper _requestMapper;
        private readonly IMockProviderRepository _mockProviderRepository;
        private readonly ILogger _logger;

        public MockProviderRequestHandler(
            IProviderServiceRequestMapper requestMapper,
            INancyResponseMapper responseMapper,
            IMockProviderRepository mockProviderRepository,
            ILogger logger)
        {
            _requestMapper = requestMapper;
            _responseMapper = responseMapper;
            _mockProviderRepository = mockProviderRepository;
            _logger = logger;
        }

        public Response Handle(NancyContext context)
        {
            return HandlePactRequest(context);
        }

        private Response HandlePactRequest(NancyContext context)
        {
            var actualRequest = _requestMapper.Convert(context.Request);
            var actualRequestMethod = actualRequest.Method.ToString().ToUpperInvariant();
            var actualRequestPath = actualRequest.Path;

            _logger.Log(String.Format("Received request {0} {1}", actualRequestMethod, actualRequestPath));
            _logger.Log(JsonConvert.SerializeObject(actualRequest, Formatting.Indented), LogType.Debug);

            ProviderServiceInteraction matchingInteraction;
            
            try
            {
                matchingInteraction = _mockProviderRepository.GetMatchingTestScopedInteraction(actualRequest);
                _mockProviderRepository.AddHandledRequest(new HandledRequest(actualRequest, matchingInteraction));

                _logger.Log(String.Format("Found matching response for {0} {1}", actualRequestMethod, actualRequestPath));
                _logger.Log(JsonConvert.SerializeObject(matchingInteraction.Response, Formatting.Indented), LogType.Debug);
            }
            catch (Exception)
            {
                _logger.Log(String.Format("No matching interaction found for {0} {1}", actualRequestMethod, actualRequestPath), LogType.Error);
                _mockProviderRepository.AddHandledRequest(new HandledRequest(actualRequest, null));
                throw;
            }
            
            return _responseMapper.Convert(matchingInteraction.Response);
        }
    }
}