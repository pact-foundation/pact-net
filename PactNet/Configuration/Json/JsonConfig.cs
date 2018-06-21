using Newtonsoft.Json;

namespace PactNet.Configuration.Json
{
    internal static class JsonConfig
    {
        private static JsonSerializerSettings _apiRequestSerializerSettings;
        internal static JsonSerializerSettings ApiSerializerSettings
        {
            get
            {
                _apiRequestSerializerSettings = _apiRequestSerializerSettings ?? new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    Formatting = Formatting.None
                };
                return _apiRequestSerializerSettings;
            }
            set { _apiRequestSerializerSettings = value; }
        }
    }
}
