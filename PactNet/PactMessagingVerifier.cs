using Newtonsoft.Json;
using PactNet.Extensions;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Models.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace PactNet
{
    public class PactMessagingVerifier : IPactMessagingVerifier
    {
        private readonly PactVerifierConfig config;
        public string PactFileUri { get; private set; }
        public PactUriOptions PactUriOptions { get; private set; }
        public string ConsumerName { get; private set; }
        public string ProviderName { get; private set; }

        public PactMessagingVerifier()
            :this(new PactVerifierConfig())
        {

        }

        public PactMessagingVerifier(PactVerifierConfig config)
        {
            this.config = config;
        }

        public IPactVerifier HonoursPactWith(string consumerName)
        {
            ConsumerName = consumerName;
            return this;
        }

        public IPactVerifier PactUri(string uri, PactUriOptions options = null)
        {
            if (String.IsNullOrWhiteSpace(uri))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            PactFileUri = uri;
            PactUriOptions = options;

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

        }

        public IPactMessagingVerifier IAmProvider(string providerName)
        {
            ProviderName = providerName;
            return this;
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

                        using (var httpClient = new HttpClient())
                        {
                            using (var response = httpClient.SendAsync(request).Result)
                            {
                                response.EnsureSuccessStatusCode();
                                pactFileJson = response.Content.ReadAsStringAsync().Result;
                            }
                        }
                    }
                }
                else //Assume it's a file uri, and we will just throw if it does not exist
                {
                    pactFileJson = File.ReadAllText(PactFileUri);
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
