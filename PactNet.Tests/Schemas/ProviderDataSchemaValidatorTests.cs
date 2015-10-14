using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NSubstitute;
using PactNet.Models;
using PactNet.Reporters;
using PactNet.Schemas.Interfaces;
using PactNet.Schemas.Models;
using PactNet.Schemas.Validators;
using Xunit;

namespace PactNet.Tests.Schemas
{
    public class SampleDocument
    {
        [JsonProperty("sampleproperty", Required = Required.Always)]
        public int SampleProperty;
    }

    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public class ProviderDataSchemaValidatorTests
    {
        private IReporter _mockReporter;

        private IProviderDataSchemaValidator GetSubject()
        {
            _mockReporter = Substitute.For<IReporter>();

            return new ProviderDataSchemaValidator(_mockReporter, new PactVerifierConfig());
        }

        [Fact]
        public void Validate_WithNullPactFile_ThrowsArgumentException()
        {
            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(null, null, null));
        }

        [Fact]
        public void Validate_WithNullConsumer_ThrowsArgumentException()
        {
            var pact = new ProviderSchemaPactFile
            {
                Provider = new Pacticipant { Name = "My Provider" }
            };

            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(pact, null, null));
        }
        
        [Fact]
        public void Validate_WithNullConsumerName_ThrowsArgumentException()
        {
            var pact = new ProviderSchemaPactFile
            {
                Consumer = new Pacticipant(),
                Provider = new Pacticipant { Name = "My Provider" }
            };

            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(pact, null, null));
        }

        [Fact]
        public void Validate_WithEmptyConsumerName_ThrowsArgumentException()
        {
            var pact = new ProviderSchemaPactFile
            {
                Consumer = new Pacticipant { Name = string.Empty },
                Provider = new Pacticipant { Name = "My Provider" }
            };

            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(pact, null, null));
        }

        [Fact]
        public void Validate_WithNullProvider_ThrowsArgumentException()
        {
            var pact = new ProviderSchemaPactFile
            {
                Consumer = new Pacticipant { Name = "My consumer" },
            };

            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(pact, null, null));
        }

        [Fact]
        public void Validate_WithNullProviderName_ThrowsArgumentException()
        {
            var pact = new ProviderSchemaPactFile
            {
                Consumer = new Pacticipant { Name = "My consumer" },
                Provider = new Pacticipant()
            };

            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(pact, null, null));
        }

        [Fact]
        public void Validate_WithEmptyProviderName_ThrowsArgumentException()
        {
            var pact = new ProviderSchemaPactFile
            {
                Consumer = new Pacticipant { Name = "My consumer" },
                Provider = new Pacticipant { Name = string.Empty },
            };

            var validator = GetSubject();

            Assert.Throws<ArgumentException>(() => validator.Validate(pact, null, null));
        }

        [Fact]
        public void Validate_WithNullSchemasInPactFile_DoesNotCallTheValidateDocumentBasedOnSchema()
        {
            var pact = new ProviderSchemaPactFile
            {
                Consumer = new Pacticipant { Name = "My consumer" },
                Provider = new Pacticipant { Name = "My Provider" }
            };

            var validator = GetSubject();

            validator.Validate(pact, null, null);
            
            _mockReporter.Received(0).ReportInfo(string.Format("Verifying a Pact between {0} and {1}", pact.Consumer.Name, pact.Provider.Name));
        }

        [Fact]
        public void Validate_WithStateSpecifiedButNotFound_ExceptionIsThrown()
        {
            var schemaGenerator = new JSchemaGenerator();
            var pact = new ProviderSchemaPactFile
            {
                Consumer = new Pacticipant { Name = "My consumer" },
                Provider = new Pacticipant { Name = "My Provider" },
                Schemas = new List<ProviderDataSchema>
                {
                    new ProviderDataSchema
                    {
                        Description = "My description",
                        ProviderState = "My provider state",
                        Schema = schemaGenerator.Generate(typeof(Sample))
                    }
                }
            };

            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("My Provider State", () => { }));

            var validator = GetSubject();
            
            Assert.Throws<InvalidOperationException>(() => validator.Validate(pact, providerStates, null));
        }

        [Fact]
        public void Validate_WithStateSpecified_InvokeProviderStateSetUpIfApplicable_Iscalled()
        {
            var schemaGenerator = new JSchemaGenerator();
            var pact = new ProviderSchemaPactFile
            {
                Consumer = new Pacticipant { Name = "My consumer" },
                Provider = new Pacticipant { Name = "My Provider" },
                Schemas = new List<ProviderDataSchema>
                {
                    new ProviderDataSchema
                    {
                        Description = "My description",
                        ProviderState = "My provider state",
                        Schema = schemaGenerator.Generate(typeof(Sample))
                    }
                }
            };

            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("My provider state", () => { }));

            var validator = GetSubject();
            
            Assert.Throws<ArgumentNullException>(() => validator.Validate(pact, providerStates, null));

            _mockReporter.Received(1).ReportInfo("Provider state setup was executed");
        }

        [Fact]
        public void Validate_WithStateSpecified_InvokeProviderStateTeardownIfApplicable_Iscalled()
        {
            var schemaGenerator = new JSchemaGenerator();
            var pact = new ProviderSchemaPactFile
            {
                Consumer = new Pacticipant { Name = "My consumer" },
                Provider = new Pacticipant { Name = "My Provider" },
                Schemas = new List<ProviderDataSchema>
                {
                    new ProviderDataSchema
                    {
                        Description = "My description",
                        ProviderState = "My provider state",
                        Schema = schemaGenerator.Generate(typeof(Sample))
                    }
                }
            };

            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("My provider state", setUp: null, tearDown: () => { }));

            var validator = GetSubject();

            Assert.Throws<ArgumentNullException>(() => validator.Validate(pact, providerStates, null));

            _mockReporter.Received(1).ReportInfo("Provider state teardown was executed");
        }

        [Fact]
        public void Validate_WithSchemaAndWrongDocumentSpecified_ValidateFails()
        {
            var schemaGenerator = new JSchemaGenerator();
            var pact = new ProviderSchemaPactFile
            {
                Consumer = new Pacticipant { Name = "My consumer" },
                Provider = new Pacticipant { Name = "My Provider" },
                Schemas = new List<ProviderDataSchema>
                {
                    new ProviderDataSchema
                    {
                        Description = "My description",
                        ProviderState = "My provider state",
                        Schema = schemaGenerator.Generate(typeof(Sample))
                    }
                }
            };

            const string documentToValidate = @"{'MissinSampleproperty': 1}";
            
            var documentToValidateJsonObject = JObject.Parse(documentToValidate);

            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("My provider state", () => { }));

            var validator = GetSubject();

            Assert.Throws<PactFailureException>(() => validator.Validate(pact, providerStates, documentToValidateJsonObject));
        }

        [Fact]
        public void Validate_WithSchemaAndWrongDocumentSpecified_ValidateIsSuccessfull()
        {
            var schemaGenerator = new JSchemaGenerator();
            var pact = new ProviderSchemaPactFile
            {
                Consumer = new Pacticipant { Name = "My consumer" },
                Provider = new Pacticipant { Name = "My Provider" },
                Schemas = new List<ProviderDataSchema>
                {
                    new ProviderDataSchema
                    {
                        Description = "My description",
                        ProviderState = "My provider state",
                        Schema = schemaGenerator.Generate(typeof(Sample))
                    }
                }
            };

            const string documentToValidate = @"{'sampleproperty': 1}";

            var documentToValidateJsonObject = JObject.Parse(documentToValidate);

            var providerStates = new ProviderStates();
            providerStates.Add(new ProviderState("My provider state", () => { }));

            var validator = GetSubject();

            validator.Validate(pact, providerStates, documentToValidateJsonObject);

            _mockReporter.Received(1).ReportInfo("Validation completed successfully");
        }
    }
}
