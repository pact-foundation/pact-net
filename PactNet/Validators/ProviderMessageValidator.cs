using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PactNet.Models.Messaging;
using PactNet.Comparers.Messaging;
using PactNet.Reporters;
using PactNet.Comparers;
using System.Globalization;
using PactNet.Logging;
using PactNet.Mocks;

namespace PactNet.Validators
{
    internal class ProviderMessageValidator : IProviderMessageValidator
    {
        private readonly MessageComparer providerMessageComparer;
        private readonly IReporter reporter;
        private readonly PactVerifierConfig config;
        private readonly IMockMessager messager;

        internal ProviderMessageValidator(
           MessageComparer providerMessageComparer,
           IReporter reporter,
           PactVerifierConfig config,
           IMockMessager messager)
        {
            this.providerMessageComparer = providerMessageComparer;
            this.reporter = reporter;
            this.config = config ?? new PactVerifierConfig();
            this.messager = messager;
        }

        public ProviderMessageValidator(
           IReporter reporter,
           PactVerifierConfig config,
           IMockMessager messager) : this(
            new MessageComparer(),
            reporter,
            config, 
            messager)
        {
        }

        public void Validate(PactMessageFile pactFile)
        {
            if (pactFile == null)
            {
                throw new ArgumentException("Please supply a non null pactFile");
            }

            if (pactFile.Consumer == null || String.IsNullOrEmpty(pactFile.Consumer.Name))
            {
                throw new ArgumentException("Please supply a non null or empty Consumer name in the pactFile");
            }

            if (pactFile.Provider == null || String.IsNullOrEmpty(pactFile.Provider.Name))
            {
                throw new ArgumentException("Please supply a non null or empty Provider name in the pactFile");
            }

            if(pactFile.Messages != null && pactFile.Messages.Any())
            {
                this.reporter.ReportInfo(String.Format("Verifying a Pact between {0} and {1}", pactFile.Consumer.Name, pactFile.Provider.Name));

            }

            //Only grab the messages that this consumer pact is interested in.
            bool ignoreCase = true;

            var comparisonResult = new ComparisonResult();

            foreach (Message m in pactFile.Messages)
            {
                if (!String.IsNullOrWhiteSpace(m.ProviderState))
                {
                    reporter.Indent();
                    reporter.ReportInfo(String.Format("Given Provider State {0}", m.ProviderState));
                }

                if (!String.IsNullOrWhiteSpace(m.Description))
                {
                    reporter.ReportInfo(String.Format("Given Description {0}", m.Description));
                }

                var exampleMessage = this.messager.GetMessageByTopicOrProviderState(m.Description, m.ProviderState);

                if(exampleMessage != null)
                { 
                    var compareResults = this.providerMessageComparer.Compare(m, exampleMessage);

                    comparisonResult.AddChildResult(compareResults);
                    reporter.Indent();
                    reporter.ReportSummary(compareResults);
                }
                else
                {
                    comparisonResult.RecordFailure(new ErrorMessageComparisonFailure(String.Format("No supported message found for provider state {0} or description {1}", m.ProviderState, m.Description)));
                }

                reporter.ResetIndentation();
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
    }
}
