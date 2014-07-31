using System;
using PactNet.Mocks.MockHttpService;

namespace PactNet
{
    public interface IPactConsumer : IDisposable
    {
        IPactConsumer ServiceConsumer(string consumerName);
        IPactConsumer HasPactWith(string providerName);
        IMockProviderService MockService(int port);
    }
}