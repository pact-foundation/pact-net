
using System;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Core;
using PactNet.PactMessage.Models;

namespace PactNet.PactMessage.Host.Commands
{
    internal class UpdateCommand : IPactMessageCommand
    {
        private readonly string _consumer;
        private readonly string _provider;
        private readonly PactConfig _pactConfig;
        private readonly MessageInteraction _messageInteraction;
	    private readonly Func<PactMessageHostConfig, IPactCoreHost> _coreHostFactory;
	    private readonly JsonSerializerSettings _jsonSerializerSettings;

	    public UpdateCommand(string consumer, string provider, PactConfig pactConfig, MessageInteraction messageInteraction, Func<PactMessageHostConfig, IPactCoreHost> coreHostFactory, JsonSerializerSettings jsonSerializerSettings)
        {
            _consumer = consumer;
            _provider = provider;
            _pactConfig = pactConfig;
	        _messageInteraction = messageInteraction;
	        _coreHostFactory = coreHostFactory;
	        _jsonSerializerSettings = jsonSerializerSettings ?? JsonConfig.ApiSerializerSettings;
		}

		public void Execute()
        {
	        var messageJson = JsonConvert.SerializeObject(_messageInteraction, _jsonSerializerSettings);

	        var arguments = $"update '{messageJson}' --consumer='{_consumer}' --provider='{_provider}' --pact-dir={FixPathForRuby(_pactConfig.PactDir)} --pact-specification-version={_pactConfig.SpecificationVersion}";
	        var coreHostConfig = new PactMessageHostConfig(_pactConfig, arguments, true);

	        _coreHostFactory(coreHostConfig).Start();
		}

	    private static string FixPathForRuby(string path)
	    {
		    return path.Replace("\\", "/");
	    }
	}
}
