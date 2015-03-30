using System;
using System.Linq;
using PactNet.Comparers;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Models;
using PactNet.Reporters;

namespace PactNet.Mocks.MockHttpService.Validators
{
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
                _reporter.ReportInfo(String.Format("Verifying a Pact between {0} and {1}", pactFile.Consumer.Name, pactFile.Provider.Name));

                var comparisonResult = new ComparisonResult();

                foreach (var interaction in pactFile.Interactions)
                {
					InvokePactSetUpIfApplicable(providerStates);
					
                    _reporter.ResetIndentation();

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

                    InvokeProviderStateSetUpIfApplicable(providerStateItem);

                    if (!String.IsNullOrEmpty(interaction.ProviderState))
                    {
                        _reporter.Indent();
                        _reporter.ReportInfo(String.Format("Given {0}", interaction.ProviderState));
                    }

                    _reporter.Indent();
                    _reporter.ReportInfo(String.Format("{0}", interaction.Description));
                        
                    if (interaction.Request != null)
                    {
                        _reporter.Indent();
                        _reporter.ReportInfo(String.Format("with {0} {1}", interaction.Request.Method.ToString().ToUpper(), interaction.Request.Path));
                    }
                        
                    try
                    {
                        var interactionComparisonResult = ValidateInteraction(interaction);
                        comparisonResult.AddChildResult(interactionComparisonResult);
                        _reporter.Indent();
                        _reporter.ReportSummary(interactionComparisonResult);
                    }
                    finally
                    {
                        InvokeProviderStateTearDownIfApplicable(providerStateItem);
                    	InvokeTearDownIfApplicable(providerStates);                        
					}
                }

                _reporter.ResetIndentation();
                _reporter.ReportFailureReasons(comparisonResult);

                if (comparisonResult.HasFailures)
                {
                    throw new PactFailureException("See output for failure details.");
                }
            }
        }

        private ComparisonResult ValidateInteraction(ProviderServiceInteraction interaction)
        {
            var expectedResponse = interaction.Response;
            var actualResponse = _httpRequestSender.Send(interaction.Request);

            return _providerServiceResponseComparer.Compare(expectedResponse, actualResponse);
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

        private void InvokeProviderStateSetUpIfApplicable(ProviderState providerState)
        {
            if (providerState != null && providerState.SetUp != null)
            {
                providerState.SetUp();
            }
        }

        private void InvokeProviderStateTearDownIfApplicable(ProviderState providerState)
        {
            if (providerState != null && providerState.TearDown != null)
            {
                providerState.TearDown();
            }
        }
    }
}
