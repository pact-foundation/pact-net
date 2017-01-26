using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Consumer.Tests.Models
{
    public class Location
    {
        public Location()
        {
            Latitude = new Coordinate();
            Longitude = new Coordinate();
        }

        [JsonProperty("latitude")]
        public Coordinate Latitude { get; set; }

        [JsonProperty("longitude")]
        public Coordinate Longitude { get; set; }
    }
}
