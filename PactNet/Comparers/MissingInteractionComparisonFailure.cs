using System;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Comparers
{
    internal class MissingInteractionComparisonFailure : ComparisonFailure
    {
        public string RequestDescription { get; private set; }

        public MissingInteractionComparisonFailure(ProviderServiceInteraction interaction)
        {
            var requestMethod = interaction.Request != null ? interaction.Request.Method.ToString().ToUpperInvariant() : "No Method";
            var requestPath = interaction.Request != null ? interaction.Request.Path : "No Path";

            RequestDescription = String.Format("{0} {1}", requestMethod, requestPath);
            Result = String.Format(
                "The interaction with description '{0}' and provider state '{1}', was not used by the test. Missing request {2}.",
                interaction.Description,
                interaction.ProviderState,
                RequestDescription);
        }
    }
}