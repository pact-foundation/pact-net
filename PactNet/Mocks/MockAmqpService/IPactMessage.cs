using System;
using PactNet.Mocks.MockAmqpService.Models;

namespace PactNet.Mocks.MockAmqpService
{
    public interface IPactMessage 
    {
        IPactMessage ExpectedToReceive(string description);
        IPactMessage Given(string providerState);
        IPactMessage With(MessageInteraction messageInteraction);
        void VerifyConsumer(Action<string> messageHandler);
    }
}
