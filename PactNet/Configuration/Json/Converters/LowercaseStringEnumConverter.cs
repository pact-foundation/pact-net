using Newtonsoft.Json.Converters;

namespace PactNet.Configuration.Json.Converters
{
    internal class LowercaseStringEnumConverter : StringEnumConverter
    {
        public LowercaseStringEnumConverter()
        {
            CamelCaseText = true;
        }
    }
}
