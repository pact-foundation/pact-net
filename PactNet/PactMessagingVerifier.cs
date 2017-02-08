using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Comparers;
using PactNet.Comparers.Messaging;
using PactNet.Extensions;
using PactNet.Logging;
using PactNet.Mocks;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockMessager;
using PactNet.Models.Messaging;
using PactNet.Reporters;
using PactNet.Validators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace PactNet
{
    public class PactMessagingVerifier : IPactMessagingVerifier
    {

        private readonly IMockMessager mockMessager;
        private readonly Func<IReporter, PactVerifierConfig, IMockMessager, IProviderMessageValidator> providerValidatorFactory;

        private readonly PactVerifierConfig config;

        private IFileSystem _fileSystem;
        private PactBrokerClient _pactBroker;

        public string PactFileUri { get; private set; }
        public string ConsumerName { get; private set; }
        public string ProviderName { get; private set; }

        internal PactMessagingVerifier(
            PactVerifierConfig config,
            IFileSystem fileSystem,
            HttpClient httpClient,
            Func<IReporter, PactVerifierConfig, IMockMessager, IProviderMessageValidator> providerValidatorFactory)
        {
            this.config = config ?? new PactVerifierConfig();
            this.mockMessager = new MockMessanger();
            this._fileSystem = fileSystem;
            this._pactBroker = new PactBrokerClient(httpClient);
            this.providerValidatorFactory = providerValidatorFactory;
        }

        public PactMessagingVerifier()
            :this(new PactVerifierConfig())
        {

        }

        public PactMessagingVerifier(PactVerifierConfig config)
            :this(config,
                new FileSystem(), 
                new HttpClient(), 
                (reporter, verifierConfig, mockMessager) => new ProviderMessageValidator( reporter, verifierConfig, mockMessager))
        {
            
        }

        public IPactVerifier HonoursPactWith(string consumerName)
        {
            if (String.IsNullOrWhiteSpace(consumerName))
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
            if (String.IsNullOrWhiteSpace(uri))
            {
                throw new ArgumentException("Please supply a non null or empty uri");
            }

            if (!string.IsNullOrEmpty(PactFileUri))
            {
                throw new ArgumentException("PactUri has already been supplied");
            }

            _pactBroker.Options = options;
            PactFileUri = uri;

            return this;
        }

        public IPactMessagingVerifier IAmProvider(string providerName)
        {
            if (String.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentException("Please supply a non null or empty providerName");
            }

            if (!String.IsNullOrEmpty(ProviderName))
            {
                throw new ArgumentException("ProviderName has already been supplied, please instantiate a new PactVerifier if you want to perform verification for a different provider");
            }

            ProviderName = providerName;
            return this;
        }

        public IPactMessagingVerifier BroadCast(string messageDescription, string providerState, dynamic exampleMessage)
        {
            this.mockMessager.Publish(messageDescription, providerState, exampleMessage);
            return this;
        }

        public void Verify(string description = null, string providerState = null)
        {
            if (string.IsNullOrEmpty(PactFileUri))
                throw new ArgumentException("PactUri has not been set.");

            foreach (var pactFile in FetchPactFiles())
            {
                try
                {
                    var loggerName = LogProvider.CurrentLogProvider.AddLogger(this.config.LogDir, ProviderName.ToLowerSnakeCase(), "{0}_verifier.log");
                    this.config.LoggerName = loggerName;

                    this.providerValidatorFactory(new Reporter(this.config), this.config, this.mockMessager)
                        .Validate(pactFile);
                }
                finally
                {
                    LogProvider.CurrentLogProvider.RemoveLogger(this.config.LoggerName);
                }
            }
        }

        private List<MessagingPactFile> FetchPactFiles()
        {
            var pactFiles = new List<MessagingPactFile>();

            try
            {
                if (PactFileUri.IsWebUri())
                {
                    var uri = new Uri(PactFileUri);

                    if (uri.AbsolutePath == "/")
                        foreach (var pactJson in this._pactBroker.GetPactsByProvider(uri, this.ProviderName))
                            pactFiles.Add(JsonConvert.DeserializeObject<MessagingPactFile>(pactJson));
                    else
                        pactFiles.Add(JsonConvert.DeserializeObject<MessagingPactFile>(this._pactBroker.GetPactFile(uri)));
                }
                else
                {
                    var pactJson = this._fileSystem.File.ReadAllText(PactFileUri);
                    pactFiles.Add(JsonConvert.DeserializeObject<MessagingPactFile>(pactJson));
                }

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Json Pact file could not be retrieved", ex);
            }

            return pactFiles;
        }

    }
}
