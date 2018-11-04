using System;
using PactNet.Matchers;
using PactNet.Matchers.Type;
using PactNet.PactMessage;
using PactNet.PactMessage.Models;
using Xunit;

namespace PactNet.Tests.IntegrationTests
{
	public class PactMessageIntegrationTests : IClassFixture<IntegrationTestsMyConsumerPact>
	{
		private readonly IPactMessageBuilder _pactMessageBuilder;
		private readonly IPactMessage _pactMessage;

		public PactMessageIntegrationTests(IntegrationTestsMyConsumerPact integrationTestsMyConsumer)
		{
			_pactMessageBuilder = integrationTestsMyConsumer.PactBuilder;
			_pactMessage = integrationTestsMyConsumer.PactMessage;
		}

		[Fact]
		public void Build_NoMessages_VerificationSucceeds()
		{
			_pactMessage.VerifyConsumer(MessageHandler);

			_pactMessageBuilder.Build();
		}

		[Fact]
		public void Build_SubscriberCannotHandleMessage_PactFailureExceptionIsThrown()
		{
			Assert.Throws<PactFailureException>(() => _pactMessage.
				ExpectedToReceive("A message that the subscriber can't handle")
				.With(new Message
				{
					Contents = new
					{
						name = "Test"
					}
				})
				.VerifyConsumer(FailedMessageHandler));
		}

		private static void FailedMessageHandler(string obj)
		{
			throw new InvalidOperationException();
		}

		private static void MessageHandler(string message)
		{
			Assert.True(message != null);
		}
	}
}
