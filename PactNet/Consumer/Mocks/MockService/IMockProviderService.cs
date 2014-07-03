using System;

namespace PactNet.Consumer.Mocks.MockService
{
    public interface IMockProviderService : IMockProvider, IDisposable
    {
        void Start();
        void Stop();
        PactInteraction DescribeInteraction();
    }
}