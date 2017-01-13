using System.Net.Http;
using System.Threading.Tasks;
using PactNet.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal interface IProviderServiceResponseMapper
    {
        Task<ProviderServiceResponse> Convert(HttpResponseMessage msg);
    }
}