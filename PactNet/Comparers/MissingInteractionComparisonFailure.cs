using System;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Comparers
{
    internal class MissingInteractionComparisonFailure : ComparisonFailure
    {
        public string RequestDescription { get; private set; }

        public MissingInteractionComparisonFailure(ProviderServiceInteraction interaction)
        {
            var requestMethod = interaction.Request?.Method.ToString().ToUpperInvariant() ?? "No Method";
            var requestPath = interaction.Request != null ? interaction.Request.Path : "No Path";

            RequestDescription = $"{requestMethod} {requestPath}";
            Result =
                $"The interaction with description '{interaction.Description}' and provider state '{interaction.ProviderState}', was not used by the test. Missing request {RequestDescription}.";
        }
    }
}