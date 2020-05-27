using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PactNet
{
    public class VersionTagSelector
    {
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public string Tag { get; }
        public bool? All { get; }
        public bool? Latest { get; }

        public VersionTagSelector(string tag, bool? all = null, bool? latest = null)
        {
            Tag = tag;
            All = all;
            Latest = latest;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, _jsonSettings).Replace("\"", "\\\"");
        }
    }
}
