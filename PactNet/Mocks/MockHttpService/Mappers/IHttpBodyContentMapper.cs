using System.Collections.Generic;
using PactNet.Models.ProviderService;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal interface IHttpBodyContentMapper
    {
        HttpBodyContent Convert(dynamic body, IDictionary<string, string> headers);
        HttpBodyContent Convert(byte[] content, IDictionary<string, string> headers);
    }
}