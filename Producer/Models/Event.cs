using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provider.Messaging.Models
{
    public class Event
    {
        public Guid EventId { get; set; }
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }
        public Location Location { get; set; }
    }
}
