using System.Collections.Generic;

namespace PactNet.Remote.Models
{
    internal interface IHttpMessage
    {
        IDictionary<string, object> Headers { get; set; }
        dynamic Body { get; set; }
    }
}