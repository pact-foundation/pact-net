using System;
using System.Collections.Generic;
using PactNet.PactMessage.Models;

namespace PactNet.PactVerification
{
	public class MessageInvoker
	{
		private readonly Dictionary<string, Action> _providerStates;
		private readonly Dictionary<string, Func<string>> _messagePublishers;

		public MessageInvoker(Dictionary<string, Action> providerStates, Dictionary<string, Func<string>> messagePublishers)
		{
			_providerStates = providerStates;
			_messagePublishers = messagePublishers;
		}

		internal string Invoke(PactMessageDescription messageDescription)
		{
			SetUpProviderStates(messageDescription.ProviderStates);
			return GetMessageInteraction(messageDescription);
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

		private string GetMessageInteraction(PactMessageDescription messageDescription)
		{
			var actualPublisher = _messagePublishers[messageDescription.Description];

			if (actualPublisher == null)
			{
				throw new ArgumentException($"The publisher action for this message description was not supplyed: {messageDescription.Description}");
			}

			return actualPublisher();
		}
	}
}
