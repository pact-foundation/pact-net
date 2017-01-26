using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provider.Messaging.Models
{
    public class Location
    {
        public Location()
        {
            Latitude = new Coordinate();
            Longitude = new Coordinate();
        }

        public Coordinate Latitude { get; set; }

        public Coordinate Longitude { get; set; }
    }
}
