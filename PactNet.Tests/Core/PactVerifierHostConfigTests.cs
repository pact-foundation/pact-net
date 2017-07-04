using System;
using PactNet.Core;
using Xunit;

namespace PactNet.Tests.Core
{
    public class PactVerifierHostConfigTests
    {
        private IPactCoreHostConfig GetSubject(Uri baseUri = null, string pactUri = "../test/pact.json", Uri providerStateSetupUri = null, PactVerifierConfig verifierConfig = null)
        {
            return new PactVerifierHostConfig(
                baseUri ?? new Uri("http://localhost:2833"), 
                pactUri,
                providerStateSetupUri,
                verifierConfig);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsTheCorrectScript()
        {
            var config = GetSubject();

            Assert.Equal("pact-provider-verifier.rb", config.Script);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsTheCorrectArgs()
        {
            var baseUri = new Uri("http://127.0.0.1");
            var pactUri = "./tester-pact/pact-file.json";
            var providerStateSetupUri = new Uri("http://127.0.0.1/states/");

            var config = GetSubject(baseUri, pactUri, providerStateSetupUri);

            var expectedArguments = BuildExpectedArguments(baseUri, pactUri, providerStateSetupUri);

            Assert.Equal(expectedArguments, config.Arguments);
        }

        [Fact]
        public void Ctor_WhenCalledWithAHttpsPactUri_SetsTheCorrectArgs()
        {
            var baseUri = new Uri("http://127.0.0.1");
            var pactUri = "https://broker:9292/test";
            var providerStateSetupUri = new Uri("http://127.0.0.1/states/");

            var config = GetSubject(baseUri, pactUri, providerStateSetupUri);

            var expectedArguments = BuildExpectedArguments(baseUri, pactUri, providerStateSetupUri);

            Assert.Equal(expectedArguments, config.Arguments);
        }

        [Fact]
        public void Ctor_WhenCalledWithNoProviderStateSetupUri_SetsTheCorrectArgs()
        {
            var baseUri = new Uri("http://127.0.0.1");
            var pactUri = "./tester-pact/pact-file.json";

            var config = GetSubject(baseUri, pactUri);

            var expectedArguments = BuildExpectedArguments(baseUri, pactUri, null);

            Assert.Equal(expectedArguments, config.Arguments);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsWaitForExitToTrue()
        {
            var config = GetSubject();

            Assert.Equal(true, config.WaitForExit);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsOutputters()
        {
            var verifierConfig = new PactVerifierConfig();
            var config = GetSubject(verifierConfig: verifierConfig);

            Assert.Equal(verifierConfig.Outputters, config.Outputters);
        }

        [Fact]
        public void Ctor_WhenVerifierConfigIsNull_SetsOutputtersToNull()
        {
            var config = GetSubject();

            Assert.Equal(null, config.Outputters);
        }

        private string BuildExpectedArguments(
            Uri baseUri, 
            string pactUri, 
            Uri providerStateSetupUri)
        {
            var providerStateOption = providerStateSetupUri != null ? $" --provider-states-setup-url {providerStateSetupUri.OriginalString}" : "";
            return $"--pact-urls \"{pactUri}\" --provider-base-url {baseUri.OriginalString}{providerStateOption}";
        }
    }
}