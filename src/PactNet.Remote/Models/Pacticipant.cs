using Newtonsoft.Json;

namespace PactNet.Remote.Models
{
    public class Pacticipant
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}