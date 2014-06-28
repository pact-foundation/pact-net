namespace Concord
{
    public class PactInteraction
    {
        public string Description { get; set; }
        public PactProviderRequest Request { get; set; }
        public PactProviderResponse Response { get; set; }
    }
}