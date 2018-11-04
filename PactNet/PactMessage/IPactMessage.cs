using System;
using PactNet.PactMessage.Models;

namespace PactNet.PactMessage
{
    public interface IPactMessage 
    {
        IPactMessage ExpectedToReceive(string description);
        IPactMessage Given(string providerState);
        IPactMessage With(Message message);
        void VerifyConsumer(Action<string> messageHandler);
    }
}
