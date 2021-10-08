using System.Collections.Generic;

namespace PactNet.AspNetCore.ProviderState
{
    public class ProviderStateInteraction
    {
        public string State { get; set; }

        public IDictionary<string, string> Params { get; set; }
    }
}
