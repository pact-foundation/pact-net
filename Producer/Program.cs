using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Provider.Messaging.Models;

namespace Provider.Messaging
{
    class Program
    {
        static void Main(string[] args)
        {
            var messenger = new Messenger();

            Coordinate latitude = new Coordinate()
            {
                Degrees = 40,
                Minutes = 41,
                Seconds = 21.2773
            };

            Coordinate longitude = new Coordinate()
            {
                Degrees = 74,
                Minutes = 2,
                Seconds = 40.2511
            };

            var eventToBroadcast = new Event()
            {
                EventId = Guid.NewGuid(),
                EventType = "It's a Parrty",
                Timestamp = DateTime.UtcNow,
                Location = new Location()
                {
                    Latitude = latitude,
                    Longitude = longitude
                }
            };

            messenger.BroadcastEvent("MyEvent.", eventToBroadcast);
        }
    }
}
