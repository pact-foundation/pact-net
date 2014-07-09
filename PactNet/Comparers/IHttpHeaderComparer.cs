using System.Collections.Generic;

namespace PactNet.Comparers
{
    public interface IHttpHeaderComparer
    {
        void Validate(IDictionary<string, string> headers1, IDictionary<string, string> headers2);
    }
}