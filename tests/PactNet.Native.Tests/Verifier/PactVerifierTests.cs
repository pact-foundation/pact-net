using System;
using System.Collections.Generic;
using FluentAssertions;
using PactNet.Native.Verifier;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Native.Tests.Verifier
{
    public class PactVerifierTests
    {
        private readonly PactVerifier verifier;

        private readonly IDictionary<string, string> verifierArgs;

        public PactVerifierTests(ITestOutputHelper output)
        {
            this.verifierArgs = new Dictionary<string, string>();

            this.verifier = new PactVerifier(this.verifierArgs, new PactVerifierConfig
            {
                Outputters = new[]
                {
                    new XUnitOutput(output)
                }
            });
        }

        [Fact]
        public void ServiceProvider_NoBasePath_SetsProviderArgs()
        {
            this.verifier.ServiceProvider("provider name", new Uri("http://example.org:8080/"));

            this.verifierArgs
                .Should().Contain("--provider-name", "provider name")
                .And.Contain("--hostname", "example.org")
                .And.Contain("--port", "8080");
        }

        [Fact]
        public void ServiceProvider_BasePath_SetsProviderArgs()
        {
            this.verifier.ServiceProvider("provider name", new Uri("http://example.org:8080/base/path"));

            this.verifierArgs
                .Should().Contain("--provider-name", "provider name")
                .And.Contain("--hostname", "example.org")
                .And.Contain("--port", "8080")
                .And.Contain("--base-path", "/base/path");
        }
    }
}
