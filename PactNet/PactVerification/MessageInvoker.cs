using System;
using System.Collections.Generic;
using PactNet.PactMessage.Models;

namespace PactNet.PactVerification
{
	public class MessageInvoker : IMessageInvoker
	{
		private readonly IDictionary<string, Action> _providerStates;
		private readonly IDictionary<string, Func<object>> _messagePublishers;

		public MessageInvoker(IDictionary<string, Action> providerStates, IDictionary<string, Func<object>> messagePublishers)
		{
			_providerStates = providerStates;
			_messagePublishers = messagePublishers;
		}

		public object Invoke(MessagePactDescription description)
		{
			if (description.ProviderStates != null)
			{
				SetUpProviderStates(description.ProviderStates);
			}
			return GetMessageInteraction(description);
		}

		private void SetUpProviderStates(IEnumerable<ProviderState> messageDescriptionProviderStates)
		{
			foreach (var providerState in messageDescriptionProviderStates)
			{
				var actualAction = _providerStates[providerState.Name];

				if (actualAction == null)
				{
					throw new ArgumentException($"The provider state that was supplyed: {providerState.Name} could not be found.");
				}

				actualAction();
			}
		}

		private object GetMessageInteraction(MessagePactDescription description)
		{
			var actualPublisher = _messagePublishers[description.Description];

			if (actualPublisher == null)
			{
				throw new ArgumentException($"The publisher action for this message description was not supplyed: {description.Description}");
			}

			return actualPublisher();
		}
	}
}
