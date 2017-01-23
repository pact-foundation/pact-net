using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Comparers;
using PactNet.Comparers.Messaging;
using PactNet.Extensions;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Models.Messaging;
using PactNet.Reporters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace PactNet
{
    public class PactMessagingVerifier : IPactMessagingVerifier
    {
        class SampleMessage
        {
            public string Description { get; set; }
            public string ProivderState { get; set; }
            public dynamic ExampleMessage { get; set; }
        }

        private IList<SampleMessage> supportedMessages;

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
            this.supportedMessages = new List<SampleMessage>();
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

            //Only grab the messages that this consumer pact is interested in.
           bool ignoreCase = true;

            var reporter = new Reporter(this.config);

            reporter.ReportInfo(String.Format("Verifying a Pact between {0} and {1}", pactFile.Consumer.Name, pactFile.Provider.Name));

            var comparisonResult = new ComparisonResult();
            
            foreach (Message m in pactFile.Messages)
            {
                if (!String.IsNullOrWhiteSpace(m.ProviderState))
                {
                    reporter.Indent();
                    reporter.ReportInfo(String.Format("Given {0}", m.ProviderState));
                }

                if (!String.IsNullOrWhiteSpace(m.Description))
                {
                    reporter.Indent();
                    reporter.ReportInfo(String.Format("Given {0}", m.Description));
                }

                bool messageFound = false;

                foreach (var example in this.supportedMessages)
                {
                    if (string.Compare(example.Description, m.Description, ignoreCase, CultureInfo.InvariantCulture) == 0 ||
                        string.Compare(example.ProivderState, m.ProviderState, ignoreCase, CultureInfo.InvariantCulture) == 0)
                    {
                        MessageComparer comparer = new MessageComparer();
                        var compareResults = comparer.Compare(m, example);

                        comparisonResult.AddChildResult(compareResults);
                        reporter.Indent();
                        reporter.ReportSummary(compareResults);

                        messageFound = true;
                    }                    
                }

                if(!messageFound)
                {
                    comparisonResult.RecordFailure(new ErrorMessageComparisonFailure(String.Format("No supported message found for provider state {0} or description {1}", m.ProviderState, m.Description)));
                }
            }

            reporter.ResetIndentation();
            reporter.ReportFailureReasons(comparisonResult);
            reporter.Flush();

            if (comparisonResult.HasFailure)
            {
                throw new PactFailureException(String.Format("See test output or {0} for failure details.",
                    !String.IsNullOrWhiteSpace(this.config.LoggerName) ? LogProvider.CurrentLogProvider.ResolveLogPath(this.config.LoggerName) : "logs"));
            }
        }

        public IPactMessagingVerifier IAmProvider(string providerName)
        {
            ProviderName = providerName;
            return this;
        }

        public IPactMessagingVerifier BroadCast(string messageDescription, string providerState, dynamic exampleMessage)
        {
            SampleMessage example = new SampleMessage()
            {
                Description = messageDescription,
                ProivderState = providerState,
                ExampleMessage = exampleMessage
            };

            this.supportedMessages.Add(example);

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
