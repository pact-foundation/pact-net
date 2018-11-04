using Newtonsoft.Json;
using PactNet.Models;

namespace PactNet.Mocks.MockAmqpService.Models
{
    public class MessageInteraction : Interaction
    {
        [JsonProperty(PropertyName = "body")]
        public dynamic Body
        {
            get; set;
        }
    }
}
