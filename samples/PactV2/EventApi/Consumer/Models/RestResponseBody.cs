using System.Collections.Generic;
using Newtonsoft.Json;

namespace Consumer.Models
{
    public class RestResponseBody
    {
        [JsonProperty(PropertyName = "_links")]
        public Dictionary<string, HypermediaLink> Links { get; set; }

        public RestResponseBody()
        {
            Links = new Dictionary<string, HypermediaLink>();
        }
    }
}