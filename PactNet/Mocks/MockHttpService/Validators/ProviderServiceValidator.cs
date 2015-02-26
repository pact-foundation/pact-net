using System;
using System.Linq;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Models;
using PactNet.Reporters;

namespace PactNet.Mocks.MockHttpService.Validators
{
    internal class Indent
    {
        public int CurrentDepth { get; private set; }
        private const string DefaultIndent = "  ";
        private string _currentIndentDepth = "";

        public Indent(int depth = 1)
        {
            for (var i = 0; i < depth; i ++)
            {
                Increment();
            }
        }

        public void Increment()
        {
            _currentIndentDepth = _currentIndentDepth + DefaultIndent;
            CurrentDepth++;
        }

        public override string ToString()
        {
            return _currentIndentDepth;
        }
    }

    internal class ProviderServiceValidator : IProviderServiceValidator
    {
        private readonly IProviderServiceResponseComparer _providerServiceResponseComparer;
        private readonly IHttpRequestSender _httpRequestSender;
        private readonly IReporter _reporter;

        internal ProviderServiceValidator(
            IProviderServiceResponseComparer providerServiceResponseComparer,
            IHttpRequestSender httpRequestSender, 
            IReporter reporter)
        {
            _providerServiceResponseComparer = providerServiceResponseComparer;
            _httpRequestSender = httpRequestSender;
            _reporter = reporter;
        }

        public ProviderServiceValidator(
            IHttpRequestSender httpRequestSender, 
            IReporter reporter) : this(
            new ProviderServiceResponseComparer(),
            httpRequestSender,
            reporter)
        {
        }

        public void Validate(ProviderServicePactFile pactFile, ProviderStates providerStates)
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

            if (pactFile.Interactions != null && pactFile.Interactions.Any())
            {
                InvokePactSetUpIfApplicable(providerStates);

                _reporter.ReportInfo(String.Format("Verifying a Pact between {0} and {1}.", pactFile.Consumer.Name, pactFile.Provider.Name));

                try //TODO: Clean this up once the validators/comparers no longer throw
                {
                    foreach (var interaction in pactFile.Interactions)
                    {
                        var indent = new Indent();

                        ProviderState providerStateItem = null;

                        if (interaction.ProviderState != null)
                        {
                            try
                            {
                                providerStateItem = providerStates.Find(interaction.ProviderState);
                            }
                            catch (Exception)
                            {
                                providerStateItem = null;
                            }

                            if (providerStateItem == null)
                            {
                                throw new InvalidOperationException(String.Format("providerState '{0}' was defined by a consumer, however could not be found. Please supply this provider state.", interaction.ProviderState));
                            }
                        }

                        InvokeInteractionSetUpIfApplicable(providerStateItem);

                        if (!String.IsNullOrEmpty(interaction.ProviderState))
                        {
                            _reporter.ReportInfo(String.Format("{0}Given {1}", indent, interaction.ProviderState));
                            indent.Increment();
                        }

                        _reporter.ReportInfo(String.Format("{0}{1}", indent, interaction.Description));
                        indent.Increment();

                        if (interaction.Request != null)
                        {
                            _reporter.ReportInfo(String.Format("{0}with {1} {2}", indent, interaction.Request.Method.ToString().ToUpper(), interaction.Request.Path));
                            indent.Increment();
                        }

                        _reporter.ReportInfo(String.Format("{0}returns a response which", indent));
                        
                        try
                        {
                            ValidateInteraction(interaction);
                        }
                        finally
                        {
                            InvokeInteractionTearDownIfApplicable(providerStateItem);
                        }
                    }

                    _reporter.ThrowIfAnyErrors();
                }
                finally 
                {
                    InvokeTearDownIfApplicable(providerStates);
                }
            }
        }

        private void ValidateInteraction(ProviderServiceInteraction interaction)
        {
            var expectedResponse = interaction.Response;
            var actualResponse = _httpRequestSender.Send(interaction.Request);

            var responseComparisonResult = _providerServiceResponseComparer.Compare(expectedResponse, actualResponse);
            _reporter.ReportComparisonResult(responseComparisonResult);
        }

        private void InvokePactSetUpIfApplicable(ProviderStates providerStates)
        {
            if (providerStates != null && providerStates.SetUp != null)
            {
                providerStates.SetUp();
            }
        }

        private void InvokeTearDownIfApplicable(ProviderStates providerStates)
        {
            if (providerStates != null && providerStates.TearDown != null)
            {
                providerStates.TearDown();
            }
        }

        private void InvokeInteractionSetUpIfApplicable(ProviderState providerState)
        {
            if (providerState != null && providerState.SetUp != null)
            {
                providerState.SetUp();
            }
        }

        private void InvokeInteractionTearDownIfApplicable(ProviderState providerState)
        {
            if (providerState != null && providerState.TearDown != null)
            {
                providerState.TearDown();
            }
        }
    }
}
