using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Consumer.Models
{
    public class Coordinate
    {
        [JsonProperty("degrees")]
        public int Degrees { get; set; }

        [JsonProperty("minutes")]
        public int Minutes { get; set; }

        [JsonProperty("seconds")]
        public double Seconds { get; set; }
    }
}
