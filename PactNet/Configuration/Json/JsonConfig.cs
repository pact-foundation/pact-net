using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PactNet.Configuration.Json
{
    public static class JsonConfig
    {
        private static JsonSerializerSettings _serializerSettings;
        public static JsonSerializerSettings SerializerSettings 
        {
            get
            {
                _serializerSettings = _serializerSettings ?? new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                };
                return _serializerSettings;
            }
        }
    }
}
