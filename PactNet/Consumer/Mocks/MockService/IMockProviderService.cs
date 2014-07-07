namespace PactNet.Consumer.Mocks.MockService
{
    public interface IMockProviderService : IMockProvider
    {
        void Start();
        void Stop();
    }
}