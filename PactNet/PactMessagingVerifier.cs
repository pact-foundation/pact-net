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
        private readonly IFileSystem fileSystem;
        private readonly HttpClient httpClient;
        private readonly Func<IReporter, PactVerifierConfig, IMockMessager, IProviderMessageValidator> providerValidatorFactory;

        private readonly PactVerifierConfig config;

        public string PactFileUri { get; private set; }
        public PactUriOptions PactUriOptions { get; private set; }
        public string ConsumerName { get; private set; }
        public string ProviderName { get; private set; }

        internal PactMessagingVerifier(IFileSystem fileSystem,
            HttpClient httpClient,
            PactVerifierConfig config,
            Func<IReporter, PactVerifierConfig, IMockMessager, IProviderMessageValidator> providerValidatorFactory)
        {
            this.httpClient = httpClient;
            this.fileSystem = fileSystem;
            this.config = config ?? new PactVerifierConfig();
            this.mockMessager = new MockMessanger();
            this.providerValidatorFactory = providerValidatorFactory;
        }

        public PactMessagingVerifier()
            :this(new PactVerifierConfig())
        {

        }

        public PactMessagingVerifier(PactVerifierConfig config)
            :this(new FileSystem(), 
                 new HttpClient(),
                 config,
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

            PactFileUri = uri;
            PactUriOptions = options;

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
            if (String.IsNullOrWhiteSpace(PactFileUri))
            {
                throw new InvalidOperationException(
                    "PactFileUri has not been set, please supply a uri using the PactUri method.");
            }

            PactMessageFile pactFile = FetchPactFile();

            var loggerName = LogProvider.CurrentLogProvider.AddLogger(this.config.LogDir, ProviderName.ToLowerSnakeCase(), "{0}_verifier.log");
            this.config.LoggerName = loggerName;

            try
            {
                this.providerValidatorFactory(new Reporter(this.config), this.config, this.mockMessager)
                    .Validate(pactFile);
            }
            finally
            {
                LogProvider.CurrentLogProvider.RemoveLogger(this.config.LoggerName);
            }

        }

        private PactMessageFile FetchPactFile()
        {
            PactMessageFile pactFile;

            try
            {
                string pactFileJson;

                if (PactFileUri.IsWebUri())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, PactFileUri))
                    {
                        request.Headers.Add("Accept", "application/json");

                        if (PactUriOptions != null)
                        {
                            request.Headers.Add("Authorization", String.Format("{0} {1}", PactUriOptions.AuthorizationScheme, PactUriOptions.AuthorizationValue));
                        }


                        using (var response = this.httpClient.SendAsync(request).Result)
                        {
                            response.EnsureSuccessStatusCode();
                            pactFileJson = response.Content.ReadAsStringAsync().Result;
                        }

                    }
                }
                else //Assume it's a file uri, and we will just throw if it does not exist
                {
                    pactFileJson = this.fileSystem.File.ReadAllText(PactFileUri);
                }

                pactFile = JsonConvert.DeserializeObject<PactMessageFile>(pactFileJson);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Json Pact file could not be retrieved using uri \'{0}\'.", PactFileUri), ex);
            }

            return pactFile;
        }

    }
}
