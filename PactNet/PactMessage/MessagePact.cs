using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Core;
using PactNet.Infrastructure.Outputters;
using PactNet.PactMessage.Host;
using PactNet.PactMessage.Host.Commands;
using PactNet.PactMessage.Models;

namespace PactNet.PactMessage
{
	public class MessagePact : IMessagePact
	{
		private IEnumerable<ProviderState> _providerStates;
		private string _description;
		public IList<MessageInteraction> MessageInteractions { get; }

		private readonly IOutputBuilder _outputBuilder;
		private readonly Func<PactMessageHostConfig, IPactCoreHost> _coreHostFactory;
		private readonly JsonSerializerSettings _jsonSerializerSettings;

		public MessagePact(JsonSerializerSettings jsonSerializerSettings = null) : this(new OutputBuilder(),
			config => new PactCoreHost<PactMessageHostConfig>(config),
			jsonSerializerSettings)
		{
		}

		internal MessagePact(IOutputBuilder outputBuilder,
			Func<PactMessageHostConfig, IPactCoreHost> coreHostFactory,
			JsonSerializerSettings jsonSerializerSettings)
		{
			MessageInteractions = new List<MessageInteraction>();

			_outputBuilder = outputBuilder;
			_coreHostFactory = coreHostFactory;
			_jsonSerializerSettings = jsonSerializerSettings;
		}

		public IMessagePact ExpectedToReceive(string description)
		{
			if (string.IsNullOrEmpty(description))
			{
				throw new ArgumentException("Please supply a non null or empty description");
			}

			_description = description;

			return this;
		}

		public IMessagePact Given(IEnumerable<ProviderState> providerStates)
		{
			_providerStates = providerStates ?? throw new ArgumentException("Please supply a non null or empty providerStates");

			return this;
		}

		public IMessagePact With(Message message)
		{
			if (message == null)
			{
				throw new ArgumentException("Please supply a non null message");
			}

			MessageInteractions.Add(new MessageInteraction
			{
				Contents = message.Contents,
				ProviderStates = _providerStates,
				Description = _description,
			});

			return this;
		}

		public void VerifyConsumer(Action<string> messageHandler)
		{
			foreach (var messageInteraction in MessageInteractions)
			{
				var reifyAction = new ReifyCommand(messageInteraction, _outputBuilder, _coreHostFactory, _jsonSerializerSettings);
				reifyAction.Execute();

				var message = _outputBuilder.Output;
				if (message.StartsWith("ERROR"))
				{
					throw new PactFailureException($"Could not parse message. core error: {message}");
				}

				try
				{
					messageHandler(message);
				}
				catch (Exception e)
				{
					throw new PactFailureException($"could not handle the message {message}", e);
				}

				_outputBuilder.Clear();
			}
		}
	}
}
