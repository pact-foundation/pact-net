using Newtonsoft.Json.Converters;

namespace PactNet.Remote.Configuration.Json.Converters
{
    public class CamelCaseStringEnumConverter : StringEnumConverter
    {
        public CamelCaseStringEnumConverter()
        {
            this.CamelCaseText = true;
        }
    }
}
