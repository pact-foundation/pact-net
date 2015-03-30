using Newtonsoft.Json;

namespace PactNet.Models
{
    internal class Pacticipant
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}