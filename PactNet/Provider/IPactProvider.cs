namespace PactNet.Provider
{
    public interface IPactProvider
    {
        IPactProvider ServiceProvider(string providerName);
        IPactProvider HonoursPactWith(string consumerName);
        IPactProvider PactUri(string uri);
    }
}