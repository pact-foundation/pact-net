using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Producer.Models
{
    public class Event
    {
        public Guid EventId { get; set; }
        public DateTime Timestamp { get; set; }
        public string EventType { get; set; }
        public Coordinate Location { get; set; }
    }
}
