using System.Collections.Generic;

namespace PactNet.Mocks.MockHttpService.Models
{
    public interface IHttpMessage
    {
        IDictionary<string, string> Headers { get; set; }
        dynamic Body { get; set; }
    }
}