using Nancy;
using PactNet.Mappers;
using PactNet.Models.ProviderService;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal interface INancyResponseMapper : IMapper<ProviderServiceResponse, Response>
    {
    }
}