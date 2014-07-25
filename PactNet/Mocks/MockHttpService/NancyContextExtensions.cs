using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public static class NancyContextExtensions
    {
        private const string PactMockRequestResponsePairsKey = "PactMockRequestResponsePairs";

        public static KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse> GetMatchingMockRequestResponsePair(this NancyContext context, HttpVerb method, string path)
        {
            if (!context.Items.ContainsKey(PactMockRequestResponsePairsKey))
            {
                throw new InvalidOperationException("No mock request/response pairs have been registered");
            }

            var requestResponsePairs = (IEnumerable<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>>)context.Items[PactMockRequestResponsePairsKey];

            var matchingRequestResponsePairs = requestResponsePairs.Where(x =>
                x.Key.Method == method &&
                x.Key.Path == path).ToList();

            if (matchingRequestResponsePairs == null || !matchingRequestResponsePairs.Any())
            {
                throw new InvalidOperationException("No matching mock request/response pair has been registered for the current request");
            }

            if (matchingRequestResponsePairs.Count() > 1)
            {
                throw new InvalidOperationException("More than one matching mock request/response pair has been registered for the current request");
            }

            return matchingRequestResponsePairs.Single();
        }

        public static void SetMockRequestResponsePairs(this NancyContext context, IEnumerable<KeyValuePair<PactProviderServiceRequest, PactProviderServiceResponse>> mockRequestResponsePairs)
        {
            context.Items[PactMockRequestResponsePairsKey] = mockRequestResponsePairs;
        }
    }
}