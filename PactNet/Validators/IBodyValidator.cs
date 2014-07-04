using Newtonsoft.Json.Linq;

namespace PactNet.Validators
{
    public interface IBodyValidator
    {
        void Validate(JToken left, JToken right);
    }
}