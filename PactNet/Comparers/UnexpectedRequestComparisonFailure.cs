using System;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Comparers
{
    internal class UnexpectedRequestComparisonFailure : ComparisonFailure
    {
        public string RequestDescription { get; private set; }

        public UnexpectedRequestComparisonFailure(ProviderServiceRequest request)
        {
            var requestMethod = request != null ? request.Method.ToString().ToUpperInvariant() : "No Method";
            var requestPath = request != null ? request.Path : "No Path";

            RequestDescription = String.Format("{0} {1}", requestMethod, requestPath);
            Result = String.Format(
                "An unexpected request {0} was seen by the mock provider service.",
                RequestDescription);
        }
    }
}