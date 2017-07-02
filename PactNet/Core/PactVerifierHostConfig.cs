using System;
using System.Collections.Generic;
using PactNet.Infrastructure.Output;

namespace PactNet.Core
{
    internal class PactVerifierHostConfig : IPactCoreHostConfig
    {
        public string Path { get; }
        public string Arguments { get; }
        public bool WaitForExit { get; }
        public IEnumerable<IOutput> Outputters { get; }

        public PactVerifierHostConfig(Uri baseUri, string pactUri, Uri providerStateSetupUri, PactVerifierConfig config)
        {
            var providerStateOption = providerStateSetupUri != null ? $" --provider-states-setup-url {providerStateSetupUri.OriginalString}" : "";

            Path = ".\\pact\\bin\\pact-provider-verifier.bat";
            Arguments = $"--pact-urls \"{FixPathForRuby(pactUri)}\" --provider-base-url {baseUri.OriginalString}{providerStateOption}";
            WaitForExit = true;
            Outputters = config?.Outputters;
        }

        private string FixPathForRuby(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}