using PactNet.Mocks.MockAmqpService.Models;

namespace PactNet.Mocks.MockAmqpService
{
    public class RabbitMqMockProviderService : IAmqpMockpProviderService
    {
        public IAmqpMockpProviderService ExpectedToPublish(string description)
        {
            throw new System.NotImplementedException();
        }

        public void VerifyPublishing()
        {
            throw new System.NotImplementedException();
        }

        public IAmqpMockpProviderService Given(string providerState)
        {
            throw new System.NotImplementedException();
        }

        public IAmqpMockpProviderService With(AmqpProviderMessage request)
        {
            throw new System.NotImplementedException();
        }

        public void Start()
        {
            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();
        }
    }
}
