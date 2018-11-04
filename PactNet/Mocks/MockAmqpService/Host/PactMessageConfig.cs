namespace PactNet.Mocks.MockAmqpService.Host
{
    public class PactMessageConfig
    {
        public PactConfig Config { get; set; }
        public string ConsumerName { get; set; }
        public string ProviderName { get; set; }
    }
}