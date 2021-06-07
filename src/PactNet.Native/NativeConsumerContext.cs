using System;

namespace PactNet
{
    /// <summary>
    /// Context for consumer interaction verification
    /// </summary>
    public class NativeConsumerContext : IConsumerContext
    {
        /// <summary>
        /// URI for the mock server
        /// </summary>
        public Uri MockServerUri { get; internal set; }
    }
}
