using System;
using System.Collections.Generic;
using Moq;
using PactNet.Verifier;
using Xunit;

namespace PactNet.Tests.Verifier
{
    public class PactBrokerPublishOptionsTests
    {
        private const string Version = "1.2.3+abcd123";

        private readonly PactBrokerPublishOptions options;

        private readonly Mock<IVerifierProvider> mockProvider;

        public PactBrokerPublishOptionsTests()
        {
            this.mockProvider = new Mock<IVerifierProvider>();

            this.options = new PactBrokerPublishOptions(this.mockProvider.Object, Version);
        }

        [Fact]
        public void ProviderTags_WhenCalled_AddsTags()
        {
            this.options.ProviderTags("a", "b", "c");

            this.Verify(tags: new[] { "a", "b", "c" });
        }

        [Fact]
        public void ProviderBranch_WhenCalled_AddsBranch()
        {
            this.options.ProviderBranch("branch");

            this.Verify(branch: "branch");
        }

        [Fact]
        public void BuildUrl_WhenCalled_AddsBuildUrl()
        {
            var uri = new Uri("https://ci.example.org/builds/12345");

            this.options.BuildUri(uri);

            this.Verify(buildUrl: uri);
        }

        private void Verify(ICollection<string> tags = null, Uri buildUrl = null, string branch = null)
        {
            this.options.Apply();

            this.mockProvider.Verify(p => p.SetPublishOptions(Version, buildUrl, tags, branch));
        }
    }
}
