using System.Net.Http;

namespace PactNet.Provider
{
    public interface IPactProvider
    {
        IPactProvider ServiceProvider(string providerName);
        IPactProvider HonoursPactWith(string consumerName, HttpClient client);
    }
}