namespace PactNet.Mocks.MockHttpService.Models
{
    public class HandledRequest
    {
        public ProviderServiceRequest ActualRequest { get; private set; }
        public ProviderServiceInteraction MatchedInteraction { get; private set; } //TODO: Maybe we dont need a call count, we can just count how many dupes
        
        public HandledRequest(ProviderServiceRequest actualRequest, ProviderServiceInteraction matchedInteraction)
        {
            ActualRequest = actualRequest;
            MatchedInteraction = matchedInteraction;
        }
    }
}