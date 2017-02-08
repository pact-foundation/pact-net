using System.Net.Http;
using PactNet.Mappers;
using PactNet.Models.ProviderService;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal interface IProviderServiceResponseMapper : IMapper<HttpResponseMessage, ProviderServiceResponse>
    {
    }
}