using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Configuration.Json.Converters;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class PactProviderServiceRequest
    {
        [JsonConverter(typeof(LowercaseStringEnumConverter))]
        public HttpVerb Method { get; set; }

        public string Path { get; set; }

        public string Query { get; set; }

        [JsonConverter(typeof(DictionaryConverter))]
        public Dictionary<string, string> Headers { get; set; }

        public dynamic Body { get; set; }

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