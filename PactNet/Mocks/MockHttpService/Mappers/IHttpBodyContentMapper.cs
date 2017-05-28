using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal interface IHttpBodyContentMapper
    {
        HttpBodyContent Convert(dynamic body, IDictionary<string, object> headers);
        HttpBodyContent Convert(byte[] content, IDictionary<string, object> headers);
    }
}