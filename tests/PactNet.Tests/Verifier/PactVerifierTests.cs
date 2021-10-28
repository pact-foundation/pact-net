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

        private readonly Mock<IVerifierArguments> verifierArgs;

        public PactVerifierTests(ITestOutputHelper output)
        {
            this.verifierArgs = new Mock<IVerifierArguments>();

            this.verifier = new PactVerifier(this.verifierArgs.Object, new PactVerifierConfig
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
            this.verifier.ServiceProvider("provider name", new Uri("http://example.org:8080/"));

            this.verifierArgs.Verify(a => a.AddOption("--provider-name", "provider name", "providerName"));
            this.verifierArgs.Verify(a => a.AddOption("--hostname", "example.org", null));
            this.verifierArgs.Verify(a => a.AddOption("--port", "8080", null));
        }

        [Fact]
        public void ServiceProvider_WithBasePath_SetsBasePathArgs()
        {
            this.verifier.ServiceProvider("provider name", new Uri("http://example.org:8080/base/path"));

            this.verifierArgs.Verify(a => a.AddOption("--base-path", "/base/path", null));
        }

        [Fact]
        public void ServiceProvider_WithoutBasePath_DoesNotSetBasePathArg()
        {
            this.verifier.ServiceProvider("provider name", new Uri("http://example.org:8080/"));

            this.verifierArgs.Verify(a => a.AddOption("--base-path", It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
