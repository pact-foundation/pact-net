using PactNet.Mocks.MockAmqpService.Models;

namespace PactNet.Mocks.MockAmqpService.Host
{
    internal interface IPactMessageHost
    {
        string Reify(MessageInteraction messageInteraction);
        void Update(MessageInteraction messageInteraction);
    }
}
