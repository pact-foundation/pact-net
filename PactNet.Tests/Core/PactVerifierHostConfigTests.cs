using System;
using System.Collections.Generic;
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

            Assert.Equal("pact-provider-verifier", config.Script);
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

            var verifierConfig = new PactVerifierConfig
            {
                PublishVerificationResults = true,
                ProviderVersion = "1.0.0"
            };

            var config = GetSubject(baseUri: baseUri, pactUri: pactUri, providerStateSetupUri: providerStateSetupUri, verifierConfig: verifierConfig);

            var expectedArguments = BuildExpectedArguments(baseUri, pactUri, providerStateSetupUri, publishVerificationResults: true, providerVersion: "1.0.0");

            Assert.Equal(expectedArguments, config.Arguments);
        }

        [Fact]
        public void Ctor_WhenCalledWithCustomHeader_SetsTheCorrectArgs()
        {
            var baseUri = new Uri("http://127.0.0.1");
            var pactUri = "./tester-pact/pact-file.json";
            var providerStateSetupUri = new Uri("http://127.0.0.1/states/");
            var customHeader = new KeyValuePair<string, string>("Authorization", "Basic VGVzdA==");

            var verifierConfig = new PactVerifierConfig
            {
                CustomHeader = customHeader,
                ProviderVersion = "1.0.0"
            };

            var config = GetSubject(baseUri: baseUri, pactUri: pactUri, providerStateSetupUri: providerStateSetupUri, verifierConfig: verifierConfig);

            var expectedArguments = BuildExpectedArguments(baseUri, pactUri, providerStateSetupUri, providerVersion: "1.0.0", customHeader: customHeader);

            Assert.Equal(expectedArguments, config.Arguments);
        }

        [Fact]
        public void Ctor_WhenCalledWithVerboseTrue_SetsTheCorrectArgs()
        {
            var baseUri = new Uri("http://127.0.0.1");
            var pactUri = "./tester-pact/pact-file.json";
            var providerStateSetupUri = new Uri("http://127.0.0.1/states/");

            var verifierConfig = new PactVerifierConfig
            {
                Verbose = true,
                ProviderVersion = "1.0.0"
            };

            var config = GetSubject(baseUri: baseUri, pactUri: pactUri, providerStateSetupUri: providerStateSetupUri, verifierConfig: verifierConfig);

            var expectedArguments = BuildExpectedArguments(baseUri, pactUri, providerStateSetupUri, providerVersion: "1.0.0", verbose: true);

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
            string providerVersion = "",
            KeyValuePair<string, string>? customHeader = null,
            bool verbose = false)
        {
            var providerStateOption = providerStateSetupUri != null ? $" --provider-states-setup-url {providerStateSetupUri.OriginalString}" : "";
            var brokerCredentials = pactUriOptions != null ? $" --broker-username \"{pactUriOptions.Username}\" --broker-password \"{pactUriOptions.Password}\"" : "";
            var publishResults = publishVerificationResults ? $" --publish-verification-results=true --provider-app-version=\"{providerVersion}\"" : string.Empty;
            var customProviderHeader = customHeader != null ?
                $" --custom-provider-header \"{customHeader.Value.Key}:{customHeader.Value.Value}\"" :
                string.Empty;
            var verboseOutput = verbose ? " --verbose true" : string.Empty;

            return $"--pact-urls \"{pactUri}\" --provider-base-url {baseUri.OriginalString}{providerStateOption}{brokerCredentials}{publishResults}{customProviderHeader}{verboseOutput}";
        }
    }
}