using System.Collections.Generic;

namespace PactNet.Models.ProviderService
{
    internal interface IHttpMessage
    {
        IDictionary<string, string> Headers { get; set; }
        dynamic Body { get; set; }
    }
}