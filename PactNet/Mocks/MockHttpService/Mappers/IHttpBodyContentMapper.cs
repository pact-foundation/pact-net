using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService.Mappers
{
    internal interface IHttpBodyContentMapper
    {
        HttpBodyContent Convert(DynamicBodyMapRequest request);
        HttpBodyContent Convert(BinaryContentMapRequest request);
    }
}