using System;
using Newtonsoft.Json;

namespace Consumer.Models
{
    public class Event
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
