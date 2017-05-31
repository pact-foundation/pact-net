using System;
using PactNet.Core;

namespace PactNet
{
    public class PactVerifier : IPactVerifier
    {
        private readonly PactVerifierConfig _config; //TODO: Do we need the config? Should we write the output to file?
        private Uri _serviceBaseUri;

        public Uri ProviderStateSetupUri { get; private set; }
        public string ConsumerName { get; private set; }
        public string ProviderName { get; private set; }
        public string PactFileUri { get; private set; }
        public PactUriOptions PactUriOptions { get; private set; }

        public PactVerifier(PactVerifierConfig config)
        {
            _config = config ?? new PactVerifierConfig();
        }

        public IPactVerifier ProviderState(Uri providerStateSetupUri)
        {
            if (providerStateSetupUri == null)
            {
                throw new ArgumentException("Please supply a non null providerStateSetupUri");
            }

            ProviderStateSetupUri = providerStateSetupUri;

            return this;
        }

        public IPactVerifier ServiceProvider(string providerName, Uri baseUri) //TODO: Should the providerStatesUri get defined here or on the ?
        {
            if (String.IsNullOrEmpty(providerName))
            {
                throw new ArgumentException("Please supply a non null or empty providerName");
            }

            if (!String.IsNullOrEmpty(ProviderName))
            {
                throw new ArgumentException("ProviderName has already been supplied, please instantiate a new PactVerifier if you want to perform verification for a different provider");
            }

            if (baseUri == null)
            {
                throw new ArgumentException("Please supply a non null baseUri");
            }

            ProviderName = providerName;
            _serviceBaseUri = baseUri;
                
            return this;
        }

        public IPactVerifier HonoursPactWith(string consumerName)
        {
            if (String.IsNullOrEmpty(consumerName))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            if (!String.IsNullOrEmpty(ConsumerName))
            {
                throw new ArgumentException("ConsumerName has already been supplied, please instantiate a new PactVerifier if you want to perform verification for a different consumer");
            }

            ConsumerName = consumerName;

            return this;
        }

        public IPactVerifier PactUri(string uri, PactUriOptions options = null)
        {
            if (String.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            PactFileUri = uri;
            PactUriOptions = options;

            return this;
        }

        public void Verify()
        {
            if (_serviceBaseUri == null)
            {
                throw new InvalidOperationException(
                    "uri has not been set, please supply a service uri using the ServiceProvider method.");
            }

            if (String.IsNullOrEmpty(PactFileUri))
            {
                throw new InvalidOperationException(
                    "PactFileUri has not been set, please supply a uri using the PactUri method.");
            }

            var processHost = new PactProcessHost<PactVerifierConfiguration>(
                new PactVerifierConfiguration(_serviceBaseUri, PactFileUri, ProviderStateSetupUri));

            processHost.Start();
        }
    }
}
