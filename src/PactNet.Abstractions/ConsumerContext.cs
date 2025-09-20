using System;

namespace PactNet
{
    /// <summary>
    /// Context for consumer interaction verification
    /// </summary>
    public class ConsumerContext : IConsumerContext
    {
        /// <summary>
        /// URI for the mock server
        /// </summary>
        public Uri MockServerUri { get; set; }
    }
}
