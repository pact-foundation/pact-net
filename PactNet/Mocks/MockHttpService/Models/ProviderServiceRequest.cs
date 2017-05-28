using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Configuration.Json.Converters;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class ProviderServiceRequest : IHttpMessage
    {
        private bool _bodyWasSet;
        private dynamic _body;

        [JsonProperty(PropertyName = "method")]
        [JsonConverter(typeof(CamelCaseStringEnumConverter))]
        public HttpVerb Method { get; set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "query")]
        public string Query { get; set; }

        [JsonProperty(PropertyName = "headers")]
        [JsonConverter(typeof(PreserveCasingDictionaryConverter))]
        public IDictionary<string, object> Headers { get; set; }

        [JsonProperty(PropertyName = "body", NullValueHandling = NullValueHandling.Include)]
        public dynamic Body
        {
            get { return _body; }
            set
            {
                _bodyWasSet = true;
                _body = value;
            }
        }

        // A not so well known feature in JSON.Net to do conditional serialization at runtime
        public bool ShouldSerializeBody()
        {
            return _bodyWasSet;
        }

        public string PathWithQuery()
        {
            if (String.IsNullOrEmpty(Path) && !String.IsNullOrEmpty(Query))
            {
                throw new InvalidOperationException("Query has been supplied, however Path has not. Please specify as Path.");
            }

            return !String.IsNullOrEmpty(Query) ?
                    String.Format("{0}?{1}", Path, Query) :
                    Path;
        }
    }
}