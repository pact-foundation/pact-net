namespace PactNet
{
    public class PactInteraction
    {
        public string Description { get; set; }
        public string ProviderState { get; set; }
        public PactProviderRequest Request { get; set; }
        public PactProviderResponse Response { get; set; }
    }
}