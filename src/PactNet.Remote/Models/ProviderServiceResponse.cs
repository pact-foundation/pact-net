using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Remote.Configuration.Json.Converters;

namespace PactNet.Remote.Models
{
    public class ProviderServiceResponse : IHttpMessage
    {
        private bool _bodyWasSet;
        private dynamic _body;

        [JsonProperty(PropertyName = "status", NullValueHandling = NullValueHandling.Ignore)]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "headers", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(PreserveCasingDictionaryConverter))]
        public IDictionary<string, object> Headers { get; set; }

        [JsonProperty(PropertyName = "body")]
        public dynamic Body
        {
            get { return this._body; }
            set
            {
                this._bodyWasSet = true;
                this._body = value;
            }
        }

        // A not so well known feature in JSON.Net to do conditional serialization at runtime
        public bool ShouldSerializeBody()
        {
            return this._bodyWasSet;
        }
    }
}