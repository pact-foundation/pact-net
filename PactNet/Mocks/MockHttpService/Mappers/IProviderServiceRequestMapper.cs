using Nancy;
using PactNet.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public interface IProviderServiceRequestMapper : IMapper<Request, ProviderServiceRequest>
    {
    }
}