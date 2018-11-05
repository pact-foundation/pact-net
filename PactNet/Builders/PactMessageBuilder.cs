using System;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Core;
using PactNet.PactMessage;
using PactNet.PactMessage.Host;
using PactNet.PactMessage.Host.Commands;
using PactNet.PactMessage.Models;
using static System.String;

namespace PactNet
{
	public class PactMessageBuilder : IPactMessageBuilder
	{
		public string ConsumerName { get; private set; }
		public string ProviderName { get; private set; }
		public PactConfig PactConfig { get; }
		private readonly Func<string, string, IPactMessage> _pactMessageFactory;
		private readonly Func<string, string, PactConfig, MessageInteraction, Func<PactMessageHostConfig, IPactCoreHost>, IPactMessageCommand> _updateCommandFactory;
		private IPactMessage _pactMessage;

		public PactMessageBuilder()
			: this(new PactConfig
			{ SpecificationVersion = "2.0.0" })
		{
		}

		public PactMessageBuilder(PactConfig config)
			: this(config, JsonConfig.ApiSerializerSettings)
		{
		}

		public PactMessageBuilder(PactConfig config, JsonSerializerSettings jsonSerializerSettings)
			: this(config, jsonSerializerSettings, (consumerName, providerName) => new PactMessageService(jsonSerializerSettings),
				(consumer, provider, pactConfig, messageInteraction, coreHostFactory)
					=> new UpdateCommand(consumer, provider, pactConfig, messageInteraction, coreHostFactory, jsonSerializerSettings))
		{
		}

		internal PactMessageBuilder(PactConfig pactConfig, JsonSerializerSettings jsonSerializerSettings, Func<string, string, IPactMessage> pactMessageFactory,
			Func<string, string, PactConfig, MessageInteraction, Func<PactMessageHostConfig, IPactCoreHost>, IPactMessageCommand> updateCommandFactory)
		{
			PactConfig = pactConfig;
			_pactMessageFactory = pactMessageFactory;
			_updateCommandFactory = updateCommandFactory;
		}

		public IPactMessageBuilder ServiceConsumer(string consumerName)
		{
			if (IsNullOrEmpty(consumerName))
			{
				throw new ArgumentException("Please supply a non null or empty consumerName");
			}

			ConsumerName = consumerName;

			return this;
		}

		public IPactMessageBuilder HasPactWith(string providerName)
		{
			if (IsNullOrEmpty(providerName))
			{
				throw new ArgumentException("Please supply a non null or empty providerName");
			}

			ProviderName = providerName;

			return this;
		}

		public void Build()
		{
			if (_pactMessage == null)
			{
				throw new InvalidOperationException("The Pact file could not be saved because the pact message is not initialised. Please initialise by calling the CreatePactMessage() method.");
			}

			foreach (var messageInteraction in _pactMessage.MessageInteractions)
			{
				var updateCommand =_updateCommandFactory(ConsumerName, ProviderName, PactConfig, messageInteraction, config => new PactCoreHost<PactMessageHostConfig>(config));
				updateCommand.Execute();
			}
		}

		public IPactMessage InitializePactMessage()
		{
			if (IsNullOrEmpty(ConsumerName))
			{
				throw new InvalidOperationException("ConsumerName has not been set, please supply a consumer name using the ServiceConsumer method.");
			}

			if (IsNullOrEmpty(ProviderName))
			{
				throw new InvalidOperationException("ProviderName has not been set, please supply a provider name using the HasPactWith method.");
			}

			_pactMessage = _pactMessageFactory(ConsumerName, ProviderName);

			return _pactMessage;
		}
	}
}
