using System;
using PactNet.Core;
using Xunit;

namespace PactNet.Tests.Core
{
    public class PactVerifierHostConfigTests
    {
        private IPactCoreHostConfig GetSubject(Uri baseUri = null, string pactUri = "../test/pact.json", Uri providerStateSetupUri = null)
        {
            return new PactVerifierHostConfig(
                baseUri ?? new Uri("http://localhost:2833"), 
                pactUri,
                providerStateSetupUri);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsTheCorrectPath()
        {
            var config = GetSubject();

            Assert.Equal(".\\pact\\bin\\pact-provider-verifier.bat", config.Path);
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

        private string BuildExpectedArguments(
            Uri baseUri, 
            string pactUri, 
            Uri providerStateSetupUri)
        {
            var providerStateOption = providerStateSetupUri != null ? $" --provider-states-url {providerStateSetupUri.OriginalString} --provider-states-setup-url {providerStateSetupUri.OriginalString}" : "";
            return $"--pact-urls \"{pactUri}\" --provider-base-url {baseUri.OriginalString}{providerStateOption}";
        }
    }
}