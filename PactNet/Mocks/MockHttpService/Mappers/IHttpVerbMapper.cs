using PactNet.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public interface IHttpVerbMapper : IMapper<string, HttpVerb>
    {
    }
}