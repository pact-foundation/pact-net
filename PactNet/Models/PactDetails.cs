using System;
using Newtonsoft.Json;
using PactNet.Extensions;

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
            return
                $"{(Consumer != null ? Consumer.Name : string.Empty)}-{(Provider != null ? Provider.Name : string.Empty)}.json"
                    .ToLowerSnakeCase();
        }
    }
}