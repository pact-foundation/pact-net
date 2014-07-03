using System;
using PactNet.Consumer.Mocks.MockService;

namespace PactNet.Consumer
{
    public interface IPactConsumer : IDisposable
    {
        IPactConsumer ServiceConsumer(string consumerName);
        IPactConsumer HasPactWith(string providerName);
        IMockProviderService MockService(int port);
    }
}