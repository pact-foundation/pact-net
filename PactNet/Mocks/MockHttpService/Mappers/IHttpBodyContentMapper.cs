using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal interface IHttpBodyContentMapper
    {
        HttpBodyContent Convert(dynamic body, IDictionary<string, string> headers);
        HttpBodyContent Convert(string content, IDictionary<string, string> headers);
    }
}