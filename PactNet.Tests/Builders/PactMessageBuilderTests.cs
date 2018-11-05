using System;
using System.Collections.Generic;
using System.Windows.Input;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.Core;
using PactNet.Core;
using PactNet.PactMessage;
using PactNet.PactMessage.Host.Commands;
using PactNet.PactMessage.Models;
using Xunit;

namespace PactNet.Tests.Builders
{
	public class PactMessageBuilderTests
	{
		private static PactMessageBuilder GetSubject()
		{
			return new PactMessageBuilder();
		}

		[Fact]
		public void ServiceConsumer_WithConsumerName_SetsConsumerName()
		{
			//Arrange
			const string consumerName = "My Service Consumer";
			var pactBuilder = GetSubject();

			//Act
			pactBuilder.ServiceConsumer(consumerName);

			//Assert
			Assert.Equal(consumerName, pactBuilder.ConsumerName);
		}

		[Fact]
		public void ServiceConsumer_WithNullConsumerName_ThrowsArgumentException()
		{
			//Arrange
			var pactBuilder = GetSubject();

			//Act + Assert
			Assert.Throws<ArgumentException>(() => pactBuilder.ServiceConsumer(null));
		}

		[Fact]
		public void ServiceConsumer_WithEmptyConsumerName_ThrowsArgumentException()
		{
			//Arrange
			var pactBuilder = GetSubject();

			//Act + Assert
			Assert.Throws<ArgumentException>(() => pactBuilder.ServiceConsumer(string.Empty));
		}

		[Fact]
		public void HasPactWith_WithProviderName_SetsProviderName()
		{
			//Arrange
			const string providerName = "My Service Provider";
			var pact = GetSubject();

			//Act
			pact.HasPactWith(providerName);

			//Assert
			Assert.Equal(providerName, pact.ProviderName);
		}

		[Fact]
		public void HasPactWith_WithNullProviderName_ThrowsArgumentException()
		{
			//Arrange
			var pactBuilder = GetSubject();

			//Act + Assert
			Assert.Throws<ArgumentException>(() => pactBuilder.HasPactWith(null));
		}

		[Fact]
		public void InitializePactMessage_WhenCalledWithoutConsumerNameSet_ThrowsInvalidOperationException()
		{
			//Arrange
			var pactBuilder = new PactMessageBuilder();

			//Act + Assert
			Assert.Throws<InvalidOperationException>(() => pactBuilder.InitializePactMessage());
		}

		[Fact]
		public void InitializePactMessage_WhenCalledWithoutProviderNameSet_ThrowsInvalidOperationException()
		{
			//Arrange
			var pactBuilder = new PactMessageBuilder();

			//Act + Assert
			Assert.Throws<InvalidOperationException>(() => pactBuilder.ServiceConsumer("Test").InitializePactMessage());
		}

		[Fact]
		public void Build_WhenCalledWithoutPactMessageInitialized_ThrowsInvalidOperationException()
		{
			//Arrange
			var pactBuilder = new PactMessageBuilder();

			//Act + Assert
			Assert.Throws<InvalidOperationException>(() => pactBuilder.Build());
		}

		[Fact]
		public void Build_WhenCalledAfterPactMessageIsInitialized_UpdatesAllMessageInteractions()
		{
			//Arrange
			var pactMessage = Substitute.For<IPactMessage>();
			var updateCommand = Substitute.For<IPactMessageCommand>();

			var expectedInteractions = new List<MessageInteraction>
				{
					new MessageInteraction
					{
						Description = "First message"
					},
					new MessageInteraction
					{
						Description = "Second message"
					},
				};

			pactMessage.MessageInteractions.Returns(expectedInteractions);

			var pactBuilder = new PactMessageBuilder(new PactConfig(), new JsonSerializerSettings(), (consumer, provider) => pactMessage,
				(consumer, provider, config, messageInteraction, hostFactory) => updateCommand);

			pactBuilder.ServiceConsumer("Test consumer").HasPactWith("Test provider").InitializePactMessage();

			//Act
			pactBuilder.Build();

			//Assert
			updateCommand.Received(2).Execute();
		}
	}
}
