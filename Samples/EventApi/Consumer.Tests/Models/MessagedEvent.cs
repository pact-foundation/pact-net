using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Consumer.Tests.Models
{
    public class MessagedEvent
    {
        [JsonProperty("eventId")]
        public Guid EventId { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }
    }
}
