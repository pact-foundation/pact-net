using System;
using System.Collections.Generic;
using PactNet.Infrastructure.Outputters;

namespace PactNet.Core
{
    internal class PactVerifierHostConfig : IPactCoreHostConfig
    {
        public string Script { get; }
        public string Arguments { get; }
        public bool WaitForExit { get; }
        public IEnumerable<IOutput> Outputters { get; }

        public PactVerifierHostConfig(Uri baseUri, string pactUri, PactUriOptions pactBrokerUriOptions, Uri providerStateSetupUri, PactVerifierConfig config)
        {
            var providerStateOption = providerStateSetupUri != null ? $" --provider-states-setup-url {providerStateSetupUri.OriginalString}" : "";
            var brokerCredentials = pactBrokerUriOptions != null ? $" --broker-user \"{pactBrokerUriOptions.Username}\" --broker-password \"{pactBrokerUriOptions.Password}\"" : "";

            Script = "pact-provider-verifier.rb";
            Arguments = $"--pact-urls \"{FixPathForRuby(pactUri)}\"{brokerCredentials} --provider-base-url {baseUri.OriginalString}{providerStateOption}";
            WaitForExit = true;
            Outputters = config?.Outputters;
        }

        private string FixPathForRuby(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}