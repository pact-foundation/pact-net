using System;
using System.IO;
using PactNet;
using PactNet.PactMessage;

namespace Consumer.Tests
{
	public class MessageConsumerEventPublisherPact :IDisposable
	{
		public IPactMessageBuilder PactBuilder { get; }
		public IPactMessage PactMessage { get; }


		public MessageConsumerEventPublisherPact()
		{
			PactBuilder = new PactMessageBuilder(new PactConfig
				{
					SpecificationVersion = "2.0.0",
					LogDir = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}logs{Path.DirectorySeparatorChar}",
					PactDir = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}"
				})
				.ServiceConsumer("Event API Message Consumer")
				.HasPactWith("Event API");

			PactMessage = PactBuilder.InitializePactMessage();
		}

		public void Dispose()
		{
			PactBuilder.Build();
		}
	}
}
