using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public static class NancyContextExtensions
    {
        private const string PactMockRequestResponsePairsKey = "PactMockRequestResponsePairs";

        public static void SetMockRequestResponsePairs(this NancyContext context, IEnumerable<KeyValuePair<ProviderServiceRequest, ProviderServiceResponse>> mockRequestResponsePairs)
        {
            context.Items[PactMockRequestResponsePairsKey] = mockRequestResponsePairs;
        }

        public static KeyValuePair<ProviderServiceRequest, ProviderServiceResponse> GetMatchingMockRequestResponsePair(this NancyContext context, HttpVerb method, string path)
        {
            if (!context.Items.ContainsKey(PactMockRequestResponsePairsKey))
            {
                throw new InvalidOperationException("No mock request/response pairs have been registered");
            }

            var requestResponsePairs = (IEnumerable<KeyValuePair<ProviderServiceRequest, ProviderServiceResponse>>)context.Items[PactMockRequestResponsePairsKey];

            var matchingRequestResponsePairs = requestResponsePairs.Where(x =>
                x.Key.Method == method &&
                x.Key.Path == path).ToList();

            if (matchingRequestResponsePairs == null || !matchingRequestResponsePairs.Any())
            {
                throw new ArgumentException("No matching mock request/response pair has been registered for the current request");
            }

            if (matchingRequestResponsePairs.Count() > 1)
            {
                throw new ArgumentException("More than one matching mock request/response pair has been registered for the current request");
            }

            return matchingRequestResponsePairs.Single();
        }
    }
}
