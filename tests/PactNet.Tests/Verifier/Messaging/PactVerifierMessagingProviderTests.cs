using FluentAssertions;
using Moq;
using PactNet.Verifier;
using PactNet.Verifier.Messaging;
using Xunit;

namespace PactNet.Tests.Verifier.Messaging
{
    public class PactVerifierMessagingProviderTests
    {
        private readonly PactVerifierMessagingProvider provider;

        private readonly Mock<IVerifierProvider> mockProvider;
        private readonly Mock<IMessageScenarios> mockScenarios;

        public PactVerifierMessagingProviderTests()
        {
            this.mockProvider = new Mock<IVerifierProvider>();
            this.mockScenarios = new Mock<IMessageScenarios>();

            this.provider = new PactVerifierMessagingProvider(this.mockProvider.Object, this.mockScenarios.Object, new PactVerifierConfig());
        }

        [Fact]
        public void WithProviderMessages_WhenCalled_AddsProviderMessages()
        {
            bool invoked = false;

            this.provider.WithProviderMessages(scenarios =>
            {
                scenarios.Should().BeSameAs(this.mockScenarios.Object);
                invoked = true;
            });

            invoked.Should().BeTrue();
        }
    }
}
