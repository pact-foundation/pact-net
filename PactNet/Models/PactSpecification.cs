using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Models
{
    public class PactSpecification
    {
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
    }
}
