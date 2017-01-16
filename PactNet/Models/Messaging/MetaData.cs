using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Models.Messaging
{
    public class MetaData
    {
        [JsonProperty(PropertyName = "contentType")]
        public string ContentType { get; set; }
    }
}
