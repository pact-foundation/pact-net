using System.Collections.Generic;

namespace PactNet
{
    /// <summary>
    /// Provider state setup request body
    /// </summary>
    public class ProviderState
    {
        /// <summary>
        /// State description
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// State parameters
        /// </summary>
        public IDictionary<string, object> Params { get; set; }
    }
}
