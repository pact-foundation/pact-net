namespace PactNet.Mocks.MockHttpService.Models
{
    public class Stat
    {
        public ProviderServiceRequest ActualRequest { get; private set; }
        public ProviderServiceInteraction MatchedInteraction { get; private set; } //Maybe we dont need a call count, we can just count how many dupes
        
        public Stat(ProviderServiceRequest actualRequest, ProviderServiceInteraction matchedInteraction)
        {
            ActualRequest = actualRequest;
            MatchedInteraction = matchedInteraction;
        }
    }
}