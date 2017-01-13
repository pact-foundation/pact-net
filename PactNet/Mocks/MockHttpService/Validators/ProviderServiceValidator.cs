using System;
using System.Linq;
using System.Threading.Tasks;
using PactNet.Comparers;
using PactNet.Logging;
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
        private readonly PactVerifierConfig _config;

        internal ProviderServiceValidator(
            IProviderServiceResponseComparer providerServiceResponseComparer,
            IHttpRequestSender httpRequestSender,
            IReporter reporter,
            PactVerifierConfig config)
        {
            _providerServiceResponseComparer = providerServiceResponseComparer;
            _httpRequestSender = httpRequestSender;
            _reporter = reporter;
            _config = config;
        }

        public ProviderServiceValidator(
            IHttpRequestSender httpRequestSender,
            IReporter reporter,
            PactVerifierConfig config) : this(
            new ProviderServiceResponseComparer(),
            httpRequestSender,
            reporter,
            config)
        {
        }

        public async Task Validate(ProviderServicePactFile pactFile, ProviderStates providerStates)
        {
            if (pactFile == null)
            {
                throw new ArgumentException("Please supply a non null pactFile");
            }

            if (string.IsNullOrEmpty(pactFile.Consumer?.Name))
            {
                throw new ArgumentException("Please supply a non null or empty Consumer name in the pactFile");
            }

            if (string.IsNullOrEmpty(pactFile.Provider?.Name))
            {
                throw new ArgumentException("Please supply a non null or empty Provider name in the pactFile");
            }

            if (pactFile.Interactions != null && pactFile.Interactions.Any())
            {
                _reporter.ReportInfo($"Verifying a Pact between {pactFile.Consumer.Name} and {pactFile.Provider.Name}");

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
                            throw new InvalidOperationException(
                                $"providerState '{interaction.ProviderState}' was defined by a consumer, however could not be found. Please supply this provider state.");
                        }
                    }

                    InvokeProviderStateSetUpIfApplicable(providerStateItem);

                    if (!string.IsNullOrEmpty(interaction.ProviderState))
                    {
                        _reporter.Indent();
                        _reporter.ReportInfo($"Given {interaction.ProviderState}");
                    }

                    _reporter.Indent();
                    _reporter.ReportInfo($"{interaction.Description}");

                    if (interaction.Request != null)
                    {
                        _reporter.Indent();
                        _reporter.ReportInfo(
                            $"with {interaction.Request.Method.ToString().ToUpper()} {interaction.Request.Path}");
                    }

                    try
                    {
                        var interactionComparisonResult = await ValidateInteraction(interaction);
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
                _reporter.Flush();

                if (comparisonResult.HasFailure)
                {
                    throw new PactFailureException(
                        $"See test output or {(!string.IsNullOrEmpty(_config.LoggerName) ? LogProvider.CurrentLogProvider.ResolveLogPath(_config.LoggerName) : "logs")} for failure details.");
                }
            }
        }

        private async Task<ComparisonResult> ValidateInteraction(ProviderServiceInteraction interaction)
        {
            var expectedResponse = interaction.Response;
            var actualResponse = await _httpRequestSender.Send(interaction.Request);

            return _providerServiceResponseComparer.Compare(expectedResponse, actualResponse);
        }

        private void InvokePactSetUpIfApplicable(ProviderStates providerStates)
        {
            providerStates?.SetUp?.Invoke();
        }

        private void InvokeTearDownIfApplicable(ProviderStates providerStates)
        {
            providerStates?.TearDown?.Invoke();
        }

        private void InvokeProviderStateSetUpIfApplicable(ProviderState providerState)
        {
            providerState?.SetUp?.Invoke();
        }

        private void InvokeProviderStateTearDownIfApplicable(ProviderState providerState)
        {
            providerState?.TearDown?.Invoke();
        }
    }
}
