using Newtonsoft.Json;

namespace PactNet.Models
{
    public class Pacticipant
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}