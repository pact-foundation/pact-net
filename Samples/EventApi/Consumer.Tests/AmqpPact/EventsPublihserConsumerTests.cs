using System;
using Consumer.Subscribers;
using PactNet.Matchers;
using PactNet.PactMessage;
using PactNet.PactMessage.Models;
using Xunit;

namespace Consumer.Tests.AmqpPact
{
	public class EventsPublihserConsumerTests : IClassFixture<MessageConsumerEventPublisherPact>
	{
		private readonly IMessagePact _messagePact;

		public EventsPublihserConsumerTests(MessageConsumerEventPublisherPact data)
		{
			_messagePact = data.MessagePact;
		}

		[Fact]
		public void EventUpdated_EventIsSavedInTheDatabase_GetsEventId()
		{
			var eventsSubscriber = new EventsSubscriber();

			var providerStates = new[]
			{
				new ProviderState
				{
					Name = "there is one event with type 'DetailsView'"
				},
				new ProviderState
				{
					Name = "Event With id 45D80D13-D5A2-48D7-8353-CBB4C0EAABF5 is in the database"
				},

			};

			_messagePact.ExpectedToReceive("Event with id 45D80D13-D5A2-48D7-8353-CBB4C0EAABF5 updated")
				.Given(providerStates)
				.With(new Message
				{
					Contents = new
					{
						eventId = Match.Type(new Guid("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"))
					}
				})
				.VerifyConsumer(messageContent => eventsSubscriber.EventUpdatedHandler(messageContent));
		}

		[Fact]
		public void EventUpdated_MultipleEventsAreSavedInTheDatabase_GetsEventId()
		{
			var eventsSubscriber = new EventsSubscriber();

			var providerStates = new[]
			{
				new ProviderState
				{
					Name = "there is no events saved in the database"
				}
			};

			_messagePact.ExpectedToReceive("Event with id 45D80D13-D5A2-48D7-8353-CBB4C0EAABF5 updated")
				.Given(providerStates)
				.With(new Message
				{
					Contents = new
					{
						eventId = Match.Type(new Guid("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"))
					}
				})
				.VerifyConsumer(messageContent => eventsSubscriber.EventUpdatedHandler(messageContent));
		}
	}
}
