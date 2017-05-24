using System;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    internal class MockProviderRequestHandler : IMockProviderRequestHandler
    {
        private readonly IResponseMapper _responseMapper;
        private readonly IProviderServiceRequestMapper _requestMapper;
        private readonly IMockProviderRepository _mockProviderRepository;
        private readonly ILog _log;

        public MockProviderRequestHandler(
            IProviderServiceRequestMapper requestMapper,
            IResponseMapper responseMapper,
            IMockProviderRepository mockProviderRepository,
            ILog log)
        {
            _requestMapper = requestMapper;
            _responseMapper = responseMapper;
            _mockProviderRepository = mockProviderRepository;
            _log = log;
        }

        public ResponseWrapper Handle(IRequestWrapper request)
        {
            return HandlePactRequest(request);
        }

        private ResponseWrapper HandlePactRequest(IRequestWrapper request)
        {
            var actualRequest = _requestMapper.Convert(request);
            var actualRequestMethod = actualRequest.Method.ToString().ToUpperInvariant();
            var actualRequestPath = actualRequest.Path;

            _log.InfoFormat("Received request {0} {1}", actualRequestMethod, actualRequestPath);
            _log.Debug(JsonConvert.SerializeObject(actualRequest, JsonConfig.PactFileSerializerSettings));

            ProviderServiceInteraction matchingInteraction;
            
            try
            {
                matchingInteraction = _mockProviderRepository.GetMatchingTestScopedInteraction(actualRequest);
                _mockProviderRepository.AddHandledRequest(new HandledRequest(actualRequest, matchingInteraction));

                _log.InfoFormat("Found matching response for {0} {1}", actualRequestMethod, actualRequestPath);
                _log.Debug(JsonConvert.SerializeObject(matchingInteraction.Response, JsonConfig.PactFileSerializerSettings));
            }
            catch (Exception)
            {
                _log.ErrorFormat("No matching interaction found for {0} {1}", actualRequestMethod, actualRequestPath);
                _mockProviderRepository.AddHandledRequest(new HandledRequest(actualRequest, null));
                throw;
            }
            
            return _responseMapper.Convert(matchingInteraction.Response);
        }
    }
}