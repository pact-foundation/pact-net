using System.Collections.Generic;

namespace PactNet.Validators
{
    public interface IHeaderValidator
    {
        void Validate(IDictionary<string, string> left, IDictionary<string, string> right);
    }
}