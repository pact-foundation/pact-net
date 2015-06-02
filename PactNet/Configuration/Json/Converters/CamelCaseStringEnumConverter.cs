using Newtonsoft.Json.Converters;

namespace PactNet.Configuration.Json.Converters
{
    public class CamelCaseStringEnumConverter : StringEnumConverter
    {
        public CamelCaseStringEnumConverter()
        {
            CamelCaseText = true;
        }
    }
}
