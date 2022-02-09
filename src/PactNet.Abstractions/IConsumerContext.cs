using System;

namespace PactNet
{
    /// <summary>
    /// Pact consumer interaction verification context
    /// </summary>
    public interface IConsumerContext
    {
        /// <summary>
        /// URI for the mock server
        /// </summary>
        Uri MockServerUri { get; }
    }
}
