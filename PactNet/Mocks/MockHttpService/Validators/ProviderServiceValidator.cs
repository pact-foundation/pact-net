using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Validators
{
    public class ProviderServiceValidator : IProviderServiceValidator
    {
        private readonly IPactProviderServiceResponseComparer _providerServiceResponseComparer;
        private readonly HttpClient _httpClient;
        private readonly IHttpRequestMessageMapper _httpRequestMessageMapper;
        private readonly IPactProviderServiceResponseMapper _pactProviderServiceResponseMapper;
        
        [Obsolete("For testing only.")]
        public ProviderServiceValidator(
            IPactProviderServiceResponseComparer providerServiceResponseComparer, 
            HttpClient httpClient, 
            IHttpRequestMessageMapper httpRequestMessageMapper,
            IPactProviderServiceResponseMapper pactProviderServiceResponseMapper)
        {
            _providerServiceResponseComparer = providerServiceResponseComparer;
            _httpClient = httpClient;
            _httpRequestMessageMapper = httpRequestMessageMapper;
            _pactProviderServiceResponseMapper = pactProviderServiceResponseMapper;
        }

        public ProviderServiceValidator(HttpClient httpClient) : this(
            new PactProviderServiceResponseComparer(), 
            httpClient,
            new HttpRequestMessageMapper(),
            new PactProviderServiceResponseMapper())
        {
        }

        public void Validate(ServicePactFile pactFile)
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
            var request = _httpRequestMessageMapper.Convert(interaction.Request);

            var response = _httpClient.SendAsync(request, CancellationToken.None).Result;

            var expectedResponse = interaction.Response;
            var actualResponse = _pactProviderServiceResponseMapper.Convert(response);

            _providerServiceResponseComparer.Compare(expectedResponse, actualResponse);
        }
    }
}
