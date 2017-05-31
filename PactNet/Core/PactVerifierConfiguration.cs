using System;

namespace PactNet.Core
{
    internal class PactVerifierConfiguration : IPactProcessConfiguration
    {
        public string Path { get; }
        public string Arguments { get; }
        public bool WaitForExit { get; }

        public PactVerifierConfiguration(Uri baseUri, string pactUri, Uri providerStateUri)
        {
            var providerStateOption = providerStateUri != null ? $" --provider-states-url {providerStateUri.OriginalString} --provider-states-setup-url {providerStateUri.OriginalString}" : "";

            Path = "C:\\src\\os\\concord\\PactNet\\Core\\pact-provider-verifier-win32\\bin\\pact-provider-verifier.bat";
            Arguments = $"--pact-urls \"{pactUri}\" --provider-base-url {baseUri}{providerStateOption}";
            WaitForExit = true;
        }
    }
}