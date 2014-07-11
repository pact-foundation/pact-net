using System.Net.Http;
using PactNet.Mappers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    public interface IHttpContentMapper : IMapper<HttpBodyContent, HttpContent>
    {
    }
}