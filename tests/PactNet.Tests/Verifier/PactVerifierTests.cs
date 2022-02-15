using System;
using Moq;
using Newtonsoft.Json;
using PactNet.Verifier;
using PactNet.Verifier.Messaging;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Tests.Verifier
{
    public class PactVerifierTests
    {
        private readonly PactVerifier verifier;

        private readonly Mock<IVerifierProvider> mockProvider;
        private readonly Mock<IMessagingProvider> mockMessaging;

        public PactVerifierTests(ITestOutputHelper output)
        {
            this.mockProvider = new Mock<IVerifierProvider>();
            this.mockMessaging = new Mock<IMessagingProvider>();

            this.verifier = new PactVerifier(this.mockProvider.Object,
                                             this.mockMessaging.Object,
                                             new PactVerifierConfig
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

        [Fact]
        public void MessagingProvider_WhenCalled_SetsProviderInfo()
        {
            var settings = new JsonSerializerSettings();
            this.mockMessaging
                .Setup(m => m.Start(settings))
                .Returns(new Uri("https://localhost:1234/pact-messaging/"));

            this.verifier.MessagingProvider("My Producer", settings);

            this.mockProvider.Verify(p => p.SetProviderInfo("My Producer", "https", "localhost", 1234, "/pact-messaging/"));
        }
    }
}
