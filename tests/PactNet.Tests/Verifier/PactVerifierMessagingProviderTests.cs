using FluentAssertions;
using Moq;
using PactNet.Verifier;
using Xunit;

namespace PactNet.Tests.Verifier
{
    public class PactVerifierMessagingProviderTests
    {
        private readonly PactVerifierMessagingProvider provider;

        private readonly Mock<IVerifierProvider> mockProvider;

        public PactVerifierMessagingProviderTests()
        {
            this.mockProvider = new Mock<IVerifierProvider>();

            this.provider = new PactVerifierMessagingProvider(this.mockProvider.Object, new PactVerifierConfig());
        }

        [Fact]
        public void WithProviderMessages_WhenCalled_AddsProviderMessages()
        {
            bool invoked = false;

            this.provider.WithProviderMessages(scenarios =>
            {
                scenarios.Should().NotBeNull();
                invoked = true;
            });

            invoked.Should().BeTrue();
        }
    }
}
