using Newtonsoft.Json;
using PactNet.Mocks.MockHttpService;
using PactNet.Models;

namespace PactNet
{
    public interface IPactBuilder
    {
        IPactBuilder ServiceConsumer(string consumerName);
        IPactBuilder HasPactWith(string providerName);
        IMockProviderService MockService(int port, bool enableSsl = false, bool enableIpv6 = false, IPAddress host = IPAddress.Loopback, string sslCert = null, string sslKey = null, bool useRemoteMockService = false);
        IMockProviderService MockService(int port, JsonSerializerSettings jsonSerializerSettings,
            bool enableSsl = false, bool enableIpv6 = false, IPAddress host = IPAddress.Loopback, string sslCert = null, string sslKey = null, bool useRemoteMockService = false);

        void Build();
    }
}
