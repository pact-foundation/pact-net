using System;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Core;
using PactNet.PactMessage;
using PactNet.PactMessage.Host;
using PactNet.PactMessage.Host.Commands;
using static System.String;

namespace PactNet
{
	public class PactMessageBuilder : IPactMessageBuilder
	{
		public string ConsumerName { get; private set; }
		public string ProviderName { get; private set; }
		public PactConfig PactConfig { get; }
		private static JsonSerializerSettings _jsonSerializerSettings;
		private readonly Func<string, string, PactMessageService> _pactMessageFactory;
		private PactMessageService _pactMessage;

		public PactMessageBuilder()
			: this(new PactConfig
			{ SpecificationVersion = "3.0.0" })
		{
		}

		public PactMessageBuilder(PactConfig config)
			: this(config, JsonConfig.ApiSerializerSettings, (consumerName, providerName) => new PactMessageService(JsonConfig.ApiSerializerSettings))
		{
		}

		public PactMessageBuilder(PactConfig config, JsonSerializerSettings jsonSerializerSettings)
			: this(config, jsonSerializerSettings, (consumerName, providerName) => new PactMessageService(_jsonSerializerSettings))
		{
		}

		internal PactMessageBuilder(PactConfig pactConfig, JsonSerializerSettings jsonSerializerSettings, Func<string, string, PactMessageService> pactMessageFactory)
		{
			if (!int.TryParse(pactConfig.SpecificationVersion.Substring(0, 1), out var specificationVersion) || specificationVersion < 3)
			{
				throw new ArgumentException("Pact message is only supported from version 3.0.0, please supply a newer specification version");
			}

			PactConfig = pactConfig;
			_jsonSerializerSettings = jsonSerializerSettings;
			_pactMessageFactory = pactMessageFactory;
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
				throw new InvalidOperationException("The Pact file could not be saved because the pact message is not initialised. Please initialise by calling the PactMessage() method.");
			}

			foreach (var messageInteraction in _pactMessage.MessageInteractions)
			{
				new UpdateCommand(ConsumerName, ProviderName, PactConfig, messageInteraction, config => new PactCoreHost<PactMessageHostConfig>(config), _jsonSerializerSettings).Execute();
			}
		}

		public IPactMessage PactMessage(JsonSerializerSettings jsonSerializerSettings)
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
