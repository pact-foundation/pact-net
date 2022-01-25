using System;
using Moq;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Tests.Verifier
{
    public class PactVerifierTests
    {
        private readonly PactVerifier verifier;

        private readonly Mock<IVerifierProvider> mockProvider;

        public PactVerifierTests(ITestOutputHelper output)
        {
            this.mockProvider = new Mock<IVerifierProvider>();

            this.verifier = new PactVerifier(this.mockProvider.Object, new PactVerifierConfig
            {
                Outputters = new[]
                {
                    new XUnitOutput(output)
                }
            });
        }

        [Fact]
        public void ServiceProvider_WhenCalled_SetsProviderArgs()
        {
            this.verifier.ServiceProvider("provider name", new Uri("http://example.org:8080/base/path"));

            this.mockProvider.Verify(p => p.SetProviderInfo("provider name", "http", "example.org", 8080, "/base/path"));
        }
    }
}
