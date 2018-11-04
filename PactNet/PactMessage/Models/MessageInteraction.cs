using Newtonsoft.Json;
using PactNet.Models;

namespace PactNet.PactMessage.Models
{
    internal class MessageInteraction : Interaction
    {
        [JsonProperty(PropertyName = "contents")]
        public dynamic Contents
        {
            get; set;
        }
	}
}
