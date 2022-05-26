using System;
using System.Collections.Generic;
using Moq;
using PactNet.Verifier;
using Xunit;

namespace PactNet.Tests.Verifier
{
    public class PactUriOptionsTests
    {
        private static readonly Uri Uri = new Uri("https://example.org/pact.json");

        private readonly PactUriOptions options;

        private readonly Mock<IVerifierProvider> mockProvider;

        public PactUriOptionsTests()
        {
            this.mockProvider = new Mock<IVerifierProvider>();

            this.options = new PactUriOptions(this.mockProvider.Object, Uri);
        }

        [Fact]
        public void Apply_NoAuthentication_SetsNoCredentials()
        {
            this.mockProvider
                .Setup(p => p.AddUrlSource(Uri, null, null, null))
                .Verifiable();

            this.options.Apply();

            this.mockProvider.Verify();
        }

        [Fact]
        public void Apply_BasicAuthentication_SetsBasicCredentials()
        {
            this.mockProvider
                .Setup(p => p.AddUrlSource(Uri, "user", "pass", null))
                .Verifiable();

            this.options.BasicAuthentication("user", "pass");
            this.options.Apply();

            this.mockProvider.Verify();
        }

        [Fact]
        public void Apply_TokenAuthentication_SetsTokenCredentials()
        {
            this.mockProvider
                .Setup(p => p.AddUrlSource(Uri, null, null, "token"))
                .Verifiable();

            this.options.TokenAuthentication("token");
            this.options.Apply();

            this.mockProvider.Verify();
        }

        [Fact]
        public void PublishResults_WithoutExtraSettings_AddsPublishArgs()
        {
            this.mockProvider.Setup(p => p.SetPublishOptions("1.2.3", null, Array.Empty<string>(), null));

            this.options.PublishResults("1.2.3");

            this.mockProvider.Verify();
        }

        [Fact]
        public void PublishResults_WithExtraSettings_AddsPublishArgs()
        {
            var buildUri = new Uri("https://ci.example.org/build/1");
            this.mockProvider.Setup(p => p.SetPublishOptions("1.2.3", buildUri, new[] { "tag1", "tag2" }, "branch"));

            this.options.PublishResults("1.2.3", settings => settings.ProviderBranch("branch")
                                                                     .ProviderTags("tag1", "tag2")
                                                                     .BuildUri(buildUri));

            this.mockProvider.Verify();
        }

        [Fact]
        public void PublishResults_ConditionMet_AddsPublishArgs()
        {
            this.mockProvider.Setup(p => p.SetPublishOptions("1.2.3", null, Array.Empty<string>(), null));

            this.options.PublishResults(true, "1.2.3");

            this.mockProvider.Verify();
        }

        [Fact]
        public void PublishResults_ConditionMet_AddsAdditionalOptions()
        {
            var buildUri = new Uri("https://ci.example.org/build/1");
            this.mockProvider.Setup(p => p.SetPublishOptions("1.2.3", buildUri, new[] { "tag1", "tag2" }, "branch"));

            this.options.PublishResults(true, "1.2.3", settings => settings.ProviderBranch("branch")
                                                                           .ProviderTags("tag1", "tag2")
                                                                           .BuildUri(buildUri));

            this.mockProvider.Verify();
        }

        [Fact]
        public void PublishResults_ConditionNotMet_DoesNotAddPublishArgs()
        {
            this.options.PublishResults(false, null, null);

            this.mockProvider.Verify(p => p.SetPublishOptions(It.IsAny<string>(),
                                                              It.IsAny<Uri>(),
                                                              It.IsAny<ICollection<string>>(),
                                                              It.IsAny<string>()),
                                     Times.Never);
        }
    }
}
