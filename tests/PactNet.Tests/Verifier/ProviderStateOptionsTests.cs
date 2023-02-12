using System;
using Moq;
using PactNet.Verifier;
using Xunit;

namespace PactNet.Tests.Verifier
{
    public class ProviderStateOptionsTests
    {
        private static readonly Uri ProviderStateUri = new Uri("https://example.org/provider-states");

        private readonly ProviderStateOptions options;

        private readonly Mock<IVerifierProvider> mockProvider;

        public ProviderStateOptionsTests()
        {
            this.mockProvider = new Mock<IVerifierProvider>();

            this.options = new ProviderStateOptions(this.mockProvider.Object, ProviderStateUri);
        }

        [Fact]
        public void WithTeardown_WhenCalled_SetsTeardown()
        {
            this.options.WithTeardown();
            this.options.Apply();
            
            this.mockProvider.Verify(p => p.SetProviderState(ProviderStateUri, true, true));
        }

        [Theory]
        [InlineData(ProviderStateStyle.Query, false)]
        [InlineData(ProviderStateStyle.Body, true)]
        public void WithStyle_WhenCalled_SetsCallStyle(ProviderStateStyle style, bool expected)
        {
            this.options.WithStyle(style);
            this.options.Apply();

            this.mockProvider.Verify(p => p.SetProviderState(ProviderStateUri, false, expected));
        }
    }
}
