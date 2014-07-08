using System;
using System.Linq;
using System.Net.Http;
using PactNet.Mappers;

namespace PactNet.Validators
{
    public class ProviderServiceValidator : IProviderServiceValidator
    {
        private readonly IPactProviderServiceResponseValidator _providerResponseValidator;
        private readonly HttpClient _httpClient;
        private readonly IHttpRequestMessageMapper _httpRequestMessageMapper;
        private readonly IPactProviderServiceResponseMapper _pactProviderResponseMapper;
        
        [Obsolete("For testing only.")]
        public ProviderServiceValidator(
            IPactProviderServiceResponseValidator providerResponseValidator, 
            HttpClient httpClient, 
            IHttpRequestMessageMapper httpRequestMessageMapper,
            IPactProviderServiceResponseMapper pactProviderResponseMapper)
        {
            _providerResponseValidator = providerResponseValidator;
            _httpClient = httpClient;
            _httpRequestMessageMapper = httpRequestMessageMapper;
            _pactProviderResponseMapper = pactProviderResponseMapper;
        }

        public ProviderServiceValidator(HttpClient httpClient) : this(
            new PactProviderServiceResponseValidator(), 
            httpClient,
            new HttpRequestMessageMapper(),
            new PactProviderServiceResponseMapper())
        {
        }

        public void Validate(ServicePactFile pactFile)
        {
            if (pactFile == null)
            {
                throw new InvalidOperationException("Please supply a non null pactFile");
            }

            if (pactFile.Interactions != null && pactFile.Interactions.Any())
            {
                var interationNumber = 1;
                foreach (var interaction in pactFile.Interactions)
                {
                    Console.WriteLine("{0}) Verifying a Pact between {1} and {2} - {3}.", interationNumber, pactFile.Consumer.Name, pactFile.Provider.Name, interaction.Description);
                    ValidateInteraction(interaction);
                    interationNumber++;
                }
            }
        }

        private void ValidateInteraction(PactServiceInteraction interaction)
        {
            var request = _httpRequestMessageMapper.Convert(interaction);

            var response = _httpClient.SendAsync(request).Result;

            var expectedResponse = interaction.Response;
            var actualResponse = _pactProviderResponseMapper.Convert(response);

            _providerResponseValidator.Validate(expectedResponse, actualResponse);
        }
    }
}
