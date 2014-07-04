namespace PactNet.Consumer.Mocks
{
    public interface IMockProvider
    {
        IMockProvider Given(string providerState);
        IMockProvider UponReceiving(string description);
        IMockProvider With(PactProviderRequest request);
        IMockProvider WillRespondWith(PactProviderResponse response);
    }
}