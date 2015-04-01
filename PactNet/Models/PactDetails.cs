using System;
using Newtonsoft.Json;

namespace PactNet.Models
{
    public class PactDetails
    {
        [JsonProperty(Order = -3, PropertyName = "provider")]
        public Pacticipant Provider { get; set; }

        [JsonProperty(Order = -2, PropertyName = "consumer")]
        public Pacticipant Consumer { get; set; }

        public string GeneratePactFileName()
        {
            return String.Format("{0}-{1}.json", 
                Consumer != null ? Consumer.Name : String.Empty,
                Provider != null ? Provider.Name : String.Empty)
                .Replace(' ', '_').ToLower();
        }
    }
}