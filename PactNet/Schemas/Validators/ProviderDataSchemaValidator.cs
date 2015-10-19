using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using PactNet.Comparers;
using PactNet.Logging;
using PactNet.Models;
using PactNet.Reporters;
using PactNet.Schemas.Interfaces;
using PactNet.Schemas.Models;

namespace PactNet.Schemas.Validators
{
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    internal class ProviderDataSchemaValidator : IProviderDataSchemaValidator
    {
        private readonly IReporter _reporter;
        private readonly PactVerifierConfig _config;

        internal ProviderDataSchemaValidator(IReporter reporter, PactVerifierConfig config)
        {
            _reporter = reporter;
            _config = config;
        }

        public void Validate(ProviderSchemaPactFile pactFile, ProviderStates providerStates, JObject documentToValidate)
        {
            if (pactFile == null)
            {
                throw new ArgumentException("Please supply a non null pactFile");
            }

            if (pactFile.Consumer == null || string.IsNullOrEmpty(pactFile.Consumer.Name))
            {
                throw new ArgumentException("Please supply a non null or empty Consumer name in the pactFile");
            }

            if (pactFile.Provider == null || string.IsNullOrEmpty(pactFile.Provider.Name))
            {
                throw new ArgumentException("Please supply a non null or empty Provider name in the pactFile");
            }

            if (pactFile.Schemas != null && pactFile.Schemas.Any())
            {
                _reporter.ReportInfo(string.Format("Verifying a Pact between {0} and {1}", pactFile.Consumer.Name, pactFile.Provider.Name));

                var comparisonResult = new ComparisonResult();

                foreach (var dataSchema in pactFile.Schemas)
                {
                    _reporter.ResetIndentation();

                    ProviderState providerStateItem = null;

                    if (dataSchema.ProviderState != null)
                    {
                        try
                        {
                            providerStateItem = providerStates.Find(dataSchema.ProviderState);
                        }
                        catch (Exception)
                        {
                            providerStateItem = null;
                        }

                        if (providerStateItem == null)
                        {
                            throw new InvalidOperationException(string.Format("providerState '{0}' was defined by a consumer, however could not be found. Please supply this provider state.", dataSchema.ProviderState));
                        }

                        _reporter.ReportInfo("Provider state was found");
                    }

                    InvokeProviderStateSetUpIfApplicable(providerStateItem);

                    if (!string.IsNullOrEmpty(dataSchema.ProviderState))
                    {
                        _reporter.Indent();
                        _reporter.ReportInfo(string.Format("Given {0}", dataSchema.ProviderState));
                    }

                    _reporter.Indent();
                    _reporter.ReportInfo(string.Format("{0}", dataSchema.Description));
                    
                    try
                    {
                        var schemaValidationResult = ValidateDocumentBasedOnSchema(dataSchema, documentToValidate);
                        comparisonResult.AddChildResult(schemaValidationResult);
                        _reporter.Indent();
                        _reporter.ReportSummary(schemaValidationResult);
                    }
                    finally
                    {
                        InvokeProviderStateTearDownIfApplicable(providerStateItem);
                    }
                }

                _reporter.ResetIndentation();
                _reporter.ReportFailureReasons(comparisonResult);
                _reporter.Flush();

                if (comparisonResult.HasFailure)
                {
                    throw new PactFailureException(string.Format("See test output or {0} for failure details.", !string.IsNullOrEmpty(_config.LoggerName) ? LogProvider.CurrentLogProvider.ResolveLogPath(_config.LoggerName) : "logs"));
                }

                _reporter.ReportInfo("Validation completed successfully");
            }
        }

        private ComparisonResult ValidateDocumentBasedOnSchema(ProviderDataSchema dateSchema, JObject documentToValidate)
        {
            IList<ValidationError> errors;

            var result = documentToValidate.IsValid(dateSchema.Schema, out errors);

            var comparisonResult = new ComparisonResult();

            if (!result)
            {
                foreach (var validationError in errors)
                {
                    comparisonResult.RecordFailure(new ErrorMessageComparisonFailure(string.Format("{0} {1}", validationError.Message, string.IsNullOrEmpty(validationError.Path) ? string.Empty : string.Format("({0})", validationError.Path))));
                }

                return comparisonResult;
            }
            
            return comparisonResult;
        }

        private void InvokeProviderStateSetUpIfApplicable(ProviderState providerState)
        {
            if (providerState != null && providerState.SetUp != null)
            {
                providerState.SetUp();
                _reporter.ReportInfo("Provider state setup was executed");
            }
        }

        private void InvokeProviderStateTearDownIfApplicable(ProviderState providerState)
        {
            if (providerState != null && providerState.TearDown != null)
            {
                providerState.TearDown();
                _reporter.ReportInfo("Provider state teardown was executed");
            }
        }
    }
}
