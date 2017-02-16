using System;
using System.Collections.Generic;
using System.Linq;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Models.ProviderService;

namespace PactNet.Mocks.MockHttpService
{
    internal class MockProviderRepository : IMockProviderRepository
    {
        private readonly IProviderServiceRequestComparer _requestComparer;

        public string TestContext { get; set; }

        private readonly List<ProviderServiceInteraction> _testScopedInteractions = new List<ProviderServiceInteraction>();
        public ICollection<ProviderServiceInteraction> TestScopedInteractions { get { return _testScopedInteractions; } }

        private readonly List<ProviderServiceInteraction> _interactions = new List<ProviderServiceInteraction>();
        public ICollection<ProviderServiceInteraction> Interactions { get { return _interactions; } }

        private readonly List<HandledRequest> _handledRequests = new List<HandledRequest>();
        public ICollection<HandledRequest> HandledRequests { get { return _handledRequests; } }

        public MockProviderRepository(IProviderServiceRequestComparer requestComparer)
        {
            _requestComparer = requestComparer;
        }

        public void AddInteraction(ProviderServiceInteraction interaction)
        {
            if (interaction == null)
            {
                throw new ArgumentNullException("interaction");
            }

            //You cannot have any duplicate interaction defined in a test scope
            if (_testScopedInteractions.Any(x => x.Description == interaction.Description &&
                x.ProviderState == interaction.ProviderState))
            {
                throw new PactFailureException(String.Format("An interaction already exists with the description '{0}' and provider state '{1}' in this test. Please supply a different description or provider state.", interaction.Description, interaction.ProviderState));
            }

            //From a Pact specification perspective, I should de-dupe any interactions that have been registered by another test as long as they match exactly!
            var duplicateInteractions = _interactions.Where(x => x.Description == interaction.Description && x.ProviderState == interaction.ProviderState).ToList();
            if (!duplicateInteractions.Any())
            {
                _interactions.Add(interaction);
            }
            else if (duplicateInteractions.Any(di => di.AsJsonString() != interaction.AsJsonString()))
            {
                //If the interaction description and provider state match, however anything else in the interaction is different, throw
                throw new PactFailureException(String.Format("An interaction registered by another test already exists with the description '{0}' and provider state '{1}', however the interaction does not match exactly. Please supply a different description or provider state. Alternatively align this interaction to match the duplicate exactly.", interaction.Description, interaction.ProviderState));
            }

            _testScopedInteractions.Add(interaction);
        }

        public void AddHandledRequest(HandledRequest handledRequest)
        {
            if (handledRequest == null)
            {
                throw new ArgumentNullException("handledRequest");
            }

            _handledRequests.Add(handledRequest);
        }

        public ProviderServiceInteraction GetMatchingTestScopedInteraction(ProviderServiceRequest request)
        {
            if (TestScopedInteractions == null || !TestScopedInteractions.Any())
            {
                throw new PactFailureException(String.Format("No interaction found for {0} {1}.", request.Method.ToString().ToUpperInvariant(), request.Path));
            }

            var matchingInteractions = new List<ProviderServiceInteraction>();

            foreach (var testScopedInteraction in TestScopedInteractions)
            {
                var requestComparisonResult = _requestComparer.Compare(testScopedInteraction.Request, request);
                if (requestComparisonResult != null && !requestComparisonResult.HasFailure)
                {
                    matchingInteractions.Add(testScopedInteraction);
                }
            }

            if (matchingInteractions == null || !matchingInteractions.Any())
            {
                throw new PactFailureException(String.Format("No interaction found for {0} {1}.", request.Method.ToString().ToUpperInvariant(), request.Path));
            }

            if (matchingInteractions.Count() > 1)
            {
                throw new PactFailureException(String.Format("More than one interaction found for {0} {1}.", request.Method.ToString().ToUpperInvariant(), request.Path));
            }

            return matchingInteractions.Single();
        }

        public void ClearTestScopedState()
        {
            _handledRequests.Clear();
            _testScopedInteractions.Clear();
            TestContext = null;
        }
    }
}