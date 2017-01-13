using System;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Comparers
{
    internal class UnexpectedRequestComparisonFailure : ComparisonFailure
    {
        public string RequestDescription { get; private set; }

        public UnexpectedRequestComparisonFailure(ProviderServiceRequest request)
        {
            var requestMethod = request?.Method.ToString().ToUpperInvariant() ?? "No Method";
            var requestPath = request != null ? request.Path : "No Path";

            RequestDescription = $"{requestMethod} {requestPath}";
            Result = $"An unexpected request {RequestDescription} was seen by the mock provider service.";
        }
    }
}