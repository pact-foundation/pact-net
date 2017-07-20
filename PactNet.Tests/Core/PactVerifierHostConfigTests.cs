using System;
using PactNet.Core;
using Xunit;

namespace PactNet.Tests.Core
{
    public class PactVerifierHostConfigTests
    {
        private IPactCoreHostConfig GetSubject(Uri baseUri = null, string pactUri = "../test/pact.json", PactUriOptions pactBrokerUriOptions = null, Uri providerStateSetupUri = null, PactVerifierConfig verifierConfig = null)
        {
            return new PactVerifierHostConfig(
                baseUri ?? new Uri("http://localhost:2833"), 
                pactUri,
                pactBrokerUriOptions,
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

            var config = GetSubject(baseUri, pactUri, null, providerStateSetupUri);

            var expectedArguments = BuildExpectedArguments(baseUri, pactUri, providerStateSetupUri);

            Assert.Equal(expectedArguments, config.Arguments);
        }

        [Fact]
        public void Ctor_WhenCalledWithAHttpPactUri_SetsTheCorrectArgs()
        {
            var baseUri = new Uri("http://127.0.0.1");
            var pactUri = "http://broker:9292/test";
            var providerStateSetupUri = new Uri("http://127.0.0.1/states/");

            var config = GetSubject(baseUri, pactUri, null, providerStateSetupUri);

            var expectedArguments = BuildExpectedArguments(baseUri, pactUri, providerStateSetupUri);

            Assert.Equal(expectedArguments, config.Arguments);
        }

        [Fact]
        public void Ctor_WhenCalledWithAAuthenticatedHttpsPactUri_SetsTheCorrectArgs()
        {
            var baseUri = new Uri("http://127.0.0.1");
            var pactUri = "https://broker:9292/test";
            var pactUriOptions = new PactUriOptions("username", "password");
            var providerStateSetupUri = new Uri("http://127.0.0.1/states/");

            var config = GetSubject(baseUri, pactUri, pactUriOptions, providerStateSetupUri);

            var expectedArguments = BuildExpectedArguments(baseUri, pactUri, providerStateSetupUri, pactUriOptions);

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
        public void Ctor_WhenCalledWithPublishVerificationResults_SetsTheCorrectArgs()
        {
            var baseUri = new Uri("http://127.0.0.1");
            var pactUri = "./tester-pact/pact-file.json";
            var providerStateSetupUri = new Uri("http://127.0.0.1/states/");

            var verifierConfig = new PactVerifierConfig();
            verifierConfig.PublishVerificationResults = true;
            verifierConfig.ProviderVersion = "1.0.0";

            var config = GetSubject(baseUri: baseUri, pactUri: pactUri, providerStateSetupUri: providerStateSetupUri, verifierConfig: verifierConfig);

            var expectedArguments = BuildExpectedArguments(baseUri, pactUri, providerStateSetupUri, publishVerificationResults: true, providerVersion: "1.0.0");

            Assert.Equal(expectedArguments, config.Arguments);
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
            Uri providerStateSetupUri,
            PactUriOptions pactUriOptions = null,
            bool publishVerificationResults = false,
            string providerVersion = "")
        {
            var providerStateOption = providerStateSetupUri != null ? $" --provider-states-setup-url {providerStateSetupUri.OriginalString}" : "";
            var brokerCredentials = pactUriOptions != null ? $" --broker-username \"{pactUriOptions.Username}\" --broker-password \"{pactUriOptions.Password}\"" : "";
            var publishResults = publishVerificationResults ? $" --publish-verification-results=true --provider-app-version=\"{providerVersion}\"" : string.Empty;

            return $"--pact-urls \"{pactUri}\" --provider-base-url {baseUri.OriginalString}{providerStateOption}{brokerCredentials}{publishResults}";
        }
    }
}