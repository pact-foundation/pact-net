using PactNet.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal interface IResponseMapper : IMapper<ProviderServiceResponse, ResponseWrapper>
    {
    }
}