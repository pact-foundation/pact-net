using System;
using System.Collections.Generic;
using PactNet.PactMessage.Models;

namespace PactNet.PactMessage
{
    public interface IPactMessage 
    {
        IPactMessage ExpectedToReceive(string description);
        IPactMessage Given(IEnumerable<ProviderState> providerStates);
        IPactMessage With(Message message);
        void VerifyConsumer(Action<string> messageHandler);
    }
}
