using System.Collections.Generic;
using PactNet.Mocks.Models;

namespace PactNet.Mocks.MockAmqpService.Models
{
    public class AmqpProviderMessage : IMessage
    {
        public IDictionary<string, object> Headers { get; set; }
        public dynamic Body { get; set; }
        public string RoutingKey { get; set; }
    }
}
