using PactNet.Matchers;
using PactNet.Matchers.Type;
using PactNet.PactMessage;
using PactNet.PactMessage.Models;
using Xunit;

namespace PactNet.Tests.IntegrationTests
{
	public class PactMessageIntegrationTests : IClassFixture<IntegrationTestsMyConsumer>
	{
		private readonly IPactMessageBuilder _pactMessageBuilder;
		private readonly IPactMessage _pactMessage;

		public PactMessageIntegrationTests(IntegrationTestsMyConsumer integrationTestsMyConsumer)
		{
			_pactMessageBuilder = integrationTestsMyConsumer.PactBuilder;
			_pactMessage = integrationTestsMyConsumer.PactMessage;
		}

		[Fact]
		public void Build_ConsumerCanHandleMessages_VerificationSucceeds()
		{
			_pactMessage.ExpectedToReceive("A message containing user details").Given("The user exists").With(new Message
			{
				Contents = new
				{
					first_name = Match.Type("Test"),
					last_name = "Test"
				}
			})
			.VerifyConsumer(MessageHandler);

			_pactMessageBuilder.Build();
		}

		private void MessageHandler(string message)
		{
			Assert.True(message != null);
		}
	}
}
