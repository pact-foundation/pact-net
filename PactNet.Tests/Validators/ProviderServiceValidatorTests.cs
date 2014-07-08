using System;
using System.Collections.Generic;
using System.Net.Http;
using NSubstitute;
using PactNet.Mappers;
using PactNet.Tests.Fakes;
using PactNet.Validators;
using Xunit;

namespace PactNet.Tests.Validators
{
    public class ProviderServiceValidatorTests
    {
        private IProviderServiceValidator GetSubject()
        {
            return new ProviderServiceValidator(new HttpClient());
        }

        [Fact]
        public void Validate_WithNullPactFile_ThrowsArgumentException()
        {
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(null));
        }

        [Fact]
        public void Validate_WithNullConsumer_ThrowsArgumentException()
        {
            var pact = new ServicePactFile
            {
                Provider = new PactParty { Name = "My Provider" }
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact));
        }

        [Fact]
        public void Validate_WithNullConsumerName_ThrowsArgumentException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty(),
                Provider = new PactParty { Name = "My Provider" }
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact));
        }

        [Fact]
        public void Validate_WithEmptyConsumerName_ThrowsArgumentException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = String.Empty },
                Provider = new PactParty { Name = "My Provider" }
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact));
        }

        [Fact]
        public void Validate_WithNullProvider_ThrowsArgumentException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact));
        }

        [Fact]
        public void Validate_WithNullProviderName_ThrowsArgumentException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty()
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact));
        }

        [Fact]
        public void Validate_WithEmptyProviderName_ThrowsArgumentException()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = String.Empty },
            };
            var providerServiceValidator = GetSubject();

            Assert.Throws<ArgumentException>(() => providerServiceValidator.Validate(pact));
        }

        [Fact]
        public void Validate_WithNullInteractionsInPactFile_DoesNotCallHttpClientOrAnyOfTheMappersOrValidators()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" }
            };
            var mockProviderServiceResponseValidator = Substitute.For<IPactProviderServiceResponseValidator>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseValidator,
                fakeHttpClient, 
                mockHttpRequestMessageMapper, 
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact);

            mockProviderServiceResponseValidator.Received(0).Validate(Arg.Any<PactProviderServiceResponse>(), Arg.Any<PactProviderServiceResponse>());
            Assert.Equal(0, fakeHttpClient.SendAsyncCallCount);
            mockHttpRequestMessageMapper.Received(0).Convert(Arg.Any<PactServiceInteraction>());
            mockPactProviderServiceResponseMapper.Received(0).Convert(Arg.Any<HttpResponseMessage>());
        }

        [Fact]
        public void Validate_WithEmptyInteractionsInPactFile_DoesNotCallHttpClientOrAnyOfTheMappersOrValidators()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>()
            };
            var mockProviderServiceResponseValidator = Substitute.For<IPactProviderServiceResponseValidator>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseValidator,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact);
            Assert.Equal(0, fakeHttpClient.SendAsyncCallCount);
            mockProviderServiceResponseValidator.Received(0).Validate(Arg.Any<PactProviderServiceResponse>(), Arg.Any<PactProviderServiceResponse>());
            mockHttpRequestMessageMapper.Received(0).Convert(Arg.Any<PactServiceInteraction>());
            mockPactProviderServiceResponseMapper.Received(0).Convert(Arg.Any<HttpResponseMessage>());
        }

        [Fact]
        public void Validate_WithInteractionsInPactFile_CallsConvertOnTheHttpRequestMessageMapper()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        Description = "My interaction"
                    }
                }
            };
            var mockProviderServiceResponseValidator = Substitute.For<IPactProviderServiceResponseValidator>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseValidator,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact);

            mockHttpRequestMessageMapper.Received(1).Convert(Arg.Any<PactServiceInteraction>());
        }

        [Fact]
        public void Validate_WithInteractionsInPactFile_CallsConvertOnTheProviderServiceResponseMapper()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        Description = "My interaction"
                    }
                }
            };
            var mockProviderServiceResponseValidator = Substitute.For<IPactProviderServiceResponseValidator>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseValidator,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact);

            mockPactProviderServiceResponseMapper.Received(1).Convert(Arg.Any<HttpResponseMessage>());
        }

        [Fact]
        public void Validate_WithInteractionsInPactFile_CallsValidateOnTheProviderServiceResponseValidator()
        {
            var pact = new ServicePactFile
            {
                Consumer = new PactParty { Name = "My client" },
                Provider = new PactParty { Name = "My Provider" },
                Interactions = new List<PactServiceInteraction>
                {
                    new PactServiceInteraction
                    {
                        Description = "My interaction"
                    }
                }
            };
            var mockProviderServiceResponseValidator = Substitute.For<IPactProviderServiceResponseValidator>();
            var fakeHttpClient = new FakeHttpClient();
            var mockHttpRequestMessageMapper = Substitute.For<IHttpRequestMessageMapper>();
            var mockPactProviderServiceResponseMapper = Substitute.For<IPactProviderServiceResponseMapper>();

            var providerServiceValidator = new ProviderServiceValidator(
                mockProviderServiceResponseValidator,
                fakeHttpClient,
                mockHttpRequestMessageMapper,
                mockPactProviderServiceResponseMapper);

            providerServiceValidator.Validate(pact);

            mockProviderServiceResponseValidator.Received(1).Validate(Arg.Any<PactProviderServiceResponse>(), Arg.Any<PactProviderServiceResponse>());
        }

    }
}
