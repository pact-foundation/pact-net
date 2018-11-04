using System;
using Xunit;

namespace PactNet.Tests
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
			const string consumerName = "My Service Consumer";
			var pactBuilder = GetSubject();

			pactBuilder.ServiceConsumer(consumerName);

			Assert.Equal(consumerName, pactBuilder.ConsumerName);
		}

		[Fact]
		public void ServiceConsumer_WithNullConsumerName_ThrowsArgumentException()
		{
			var pactBuilder = GetSubject();

			Assert.Throws<ArgumentException>(() => pactBuilder.ServiceConsumer(null));
		}

		[Fact]
		public void ServiceConsumer_WithEmptyConsumerName_ThrowsArgumentException()
		{
			var pactBuilder = GetSubject();

			Assert.Throws<ArgumentException>(() => pactBuilder.ServiceConsumer(string.Empty));
		}

		[Fact]
		public void HasPactWith_WithProviderName_SetsProviderName()
		{
			const string providerName = "My Service Provider";
			var pact = GetSubject();

			pact.HasPactWith(providerName);

			Assert.Equal(providerName, pact.ProviderName);
		}

		[Fact]
		public void HasPactWith_WithNullProviderName_ThrowsArgumentException()
		{
			var pactBuilder = GetSubject();

			Assert.Throws<ArgumentException>(() => pactBuilder.HasPactWith(null));
		}

		[Fact]
		public void PactMessage_WhenCalledWithoutConsumerNameSet_ThrowsInvalidOperationException()
		{
			var pactBuilder = new PactMessageBuilder();

			Assert.Throws<ArgumentException>(() => pactBuilder.HasPactWith(string.Empty));
		}

		[Fact]
		public void HasPactWith_WithEmptyProviderName_ThrowsArgumentException()
		{
			var pactBuilder = GetSubject();

			Assert.Throws<ArgumentException>(() => pactBuilder.HasPactWith(string.Empty));
		}
	}
}
