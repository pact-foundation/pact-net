using Newtonsoft.Json.Converters;

namespace PactNet.Configuration.Json.Converters
{
    public class LowercaseStringEnumConverter : StringEnumConverter
    {
        public LowercaseStringEnumConverter()
        {
            CamelCaseText = true;
        }
    }
}
