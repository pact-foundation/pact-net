using Newtonsoft.Json;

namespace PactNet.Remote.Models
{
    public class PactDetails
    {
        [JsonProperty(Order = -3, PropertyName = "provider")]
        public Pacticipant Provider { get; set; }

        [JsonProperty(Order = -2, PropertyName = "consumer")]
        public Pacticipant Consumer { get; set; }
    }
}