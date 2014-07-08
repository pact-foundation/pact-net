using System.Net.Http;

namespace PactNet.Mappers
{
    public interface IPactProviderServiceResponseMapper
    {
        PactProviderServiceResponse Convert(HttpResponseMessage from);
    }
}