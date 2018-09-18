using PactNet.Mocks.MockAmqpService.Models;

namespace PactNet.Mocks.MockAmqpService
{
    public interface IAmqpMockpProviderService : IMockProvider<IAmqpMockpProviderService, AmqpProviderMessage>
    {
        IAmqpMockpProviderService ExpectedToPublish(string description);
        void VerifyPublishing();
    }
}
