using System;

namespace PactNet.Core
{
    internal class PactVerifierHostConfig : IPactCoreHostConfig
    {
        public string Path { get; }
        public string Arguments { get; }
        public bool WaitForExit { get; }

        public PactVerifierHostConfig(Uri baseUri, string pactUri, Uri providerStateSetupUri)
        {
            var providerStateOption = providerStateSetupUri != null ? $" --provider-states-url {providerStateSetupUri.OriginalString} --provider-states-setup-url {providerStateSetupUri.OriginalString}" : "";

            Path = ".\\pact-provider-verifier-win32\\bin\\pact-provider-verifier.bat";
            Arguments = $"--pact-urls \"{FixPathForRuby(pactUri)}\" --provider-base-url {baseUri.OriginalString}{providerStateOption}";
            WaitForExit = true;
        }

        private string FixPathForRuby(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}