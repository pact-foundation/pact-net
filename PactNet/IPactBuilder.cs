using Newtonsoft.Json;
using PactNet.Mocks.MockHttpService;

namespace PactNet
{
    public interface IPactBuilder
    {
        IPactBuilder ServiceConsumer(string consumerName);
        IPactBuilder HasPactWith(string providerName);
        IMockProviderService MockService(int port, bool enableSsl = false, bool bindOnAllAdapters = false);
        IMockProviderService MockService(int port, JsonSerializerSettings jsonSerializerSettings, bool enableSsl = false, bool bindOnAllAdapters = false);        
        void Build();
    }
}