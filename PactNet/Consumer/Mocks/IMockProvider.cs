namespace PactNet.Consumer.Mocks
{
    public interface IMockProvider
    {
        IMockProvider UponReceiving(string description);
        IMockProvider With(PactProviderRequest request);
        IMockProvider WillRespondWith(PactProviderResponse response);
    }
}