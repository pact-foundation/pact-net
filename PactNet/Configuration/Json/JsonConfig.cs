using Newtonsoft.Json;

namespace PactNet.Configuration.Json
{
    public static class JsonConfig
    {
        private static JsonSerializerSettings _serializerSettings;
        public static JsonSerializerSettings PactFileSerializerSettings 
        {
            get
            {
                _serializerSettings = _serializerSettings ?? new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                };
                return _serializerSettings;
            }
        }
    }
}
