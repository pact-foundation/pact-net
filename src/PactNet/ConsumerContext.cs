using System;

namespace PactNet
{
    /// <summary>
    /// Context for consumer interaction verification
    /// </summary>
    internal class ConsumerContext : IConsumerContext
    {
        /// <summary>
        /// URI for the mock server
        /// </summary>
        public Uri MockServerUri { get; internal set; }
    }
}
