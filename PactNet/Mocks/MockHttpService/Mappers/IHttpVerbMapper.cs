using PactNet.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal interface IHttpVerbMapper : IMapper<string, HttpVerb>
    {
    }
}