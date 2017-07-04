using System;
using System.Collections.Generic;
using PactNet.Infrastructure.Outputters;

namespace PactNet.Core
{
    internal class PactVerifierHostConfig : IPactCoreHostConfig
    {
        public string Path { get; }
        public string Arguments { get; }
        public bool WaitForExit { get; }
        public IDictionary<string, string> EnvironmentVariables { get; }
        public IEnumerable<IOutput> Outputters { get; }

        public PactVerifierHostConfig(Uri baseUri, string pactUri, Uri providerStateSetupUri, PactVerifierConfig config)
        {
            var providerStateOption = providerStateSetupUri != null ? $" --provider-states-setup-url {providerStateSetupUri.OriginalString}" : "";

            Path = ".\\pact\\bin\\pact-provider-verifier.bat";
            Arguments = $"--pact-urls \"{FixPathForRuby(pactUri)}\" --provider-base-url {baseUri.OriginalString}{providerStateOption}";
            WaitForExit = true;
            Outputters = config?.Outputters;
            EnvironmentVariables = new Dictionary<string, string>();
        }

        private string FixPathForRuby(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}