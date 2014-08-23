using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public static class NancyContextExtensions
    {
        public static void SetMockInteraction(this NancyContext context, IEnumerable<ProviderServiceInteraction> interactions)
        {
            context.Items[Constants.PactMockInteractionsKey] = interactions;
        }

        public static ProviderServiceInteraction GetMatchingInteraction(this NancyContext context, HttpVerb method, string path)
        {
            if (!context.Items.ContainsKey(Constants.PactMockInteractionsKey))
            {
                throw new InvalidOperationException("No mock interactions have been registered");
            }

            var interactions = (IEnumerable<ProviderServiceInteraction>)context.Items[Constants.PactMockInteractionsKey];

            if (interactions == null)
            {
                throw new InvalidOperationException("No matching mock interaction has been registered for the current request");
            }

            var matchingInteractions = interactions.Where(x =>
                x.Request.Method == method &&
                x.Request.Path == path).ToList();

            if (matchingInteractions == null || !matchingInteractions.Any())
            {
                throw new InvalidOperationException("No matching mock interaction has been registered for the current request");
            }

            if (matchingInteractions.Count() > 1)
            {
                throw new InvalidOperationException("More than one matching mock interaction has been registered for the current request");
            }

            return matchingInteractions.Single();
        }
    }
}
