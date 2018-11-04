using PactNet.PactMessage;

namespace PactNet.Tests.IntegrationTests
{
	public class IntegrationTestsMyConsumer
	{
		public IPactMessageBuilder PactBuilder { get; }
		public IPactMessage PactMessage { get; }

		public IntegrationTestsMyConsumer()
		{
			PactBuilder = new PactMessageBuilder()
				.HasPactWith("His provider")
				.ServiceConsumer("My consumer");

			PactMessage = PactBuilder.PactMessage();
		}
	}
}
