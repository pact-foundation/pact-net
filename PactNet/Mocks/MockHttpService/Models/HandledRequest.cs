namespace PactNet.Mocks.MockHttpService.Models
{
    internal class HandledRequest
    {
        public ProviderServiceRequest ActualRequest { get; private set; }
        public ProviderServiceInteraction MatchedInteraction { get; private set; }
        
        public HandledRequest(ProviderServiceRequest actualRequest, ProviderServiceInteraction matchedInteraction)
        {
            ActualRequest = actualRequest;
            MatchedInteraction = matchedInteraction;
        }
    }
}