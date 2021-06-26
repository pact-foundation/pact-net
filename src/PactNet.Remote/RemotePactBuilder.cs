using System;
using System.Threading.Tasks;

namespace PactNet.Remote
{
    /// <summary>
    /// Pact builder for an existing remote server
    /// </summary>
    public class RemotePactBuilder : IPactBuilderV2, IPactBuilderV3
    {
        private readonly string consumer;
        private readonly string provider;
        private readonly Uri uri;
        private readonly PactConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="RemotePactBuilder"/> class.
        /// </summary>
        /// <param name="consumer">Consumer name</param>
        /// <param name="provider">Provider name</param>
        /// <param name="uri">Remote server URI</param>
        /// <param name="config">Configuration</param>
        internal RemotePactBuilder(string consumer, string provider, Uri uri, PactConfig config)
        {
            this.consumer = consumer;
            this.provider = provider;
            this.uri = uri;
            this.config = config;
        }

        /// <summary>
        /// Add a new interaction to the pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IPactBuilderV2.UponReceiving(string description)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a new interaction to the pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IPactBuilderV3.UponReceiving(string description)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verify the configured interactions
        /// </summary>
        /// <param name="interact">Action to perform the real interactions against the mock server</param>
        /// <exception cref="PactFailureException">Failed to verify the interactions</exception>
        public void Verify(Action<IConsumerContext> interact)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verify the configured interactions
        /// </summary>
        /// <param name="interact">Action to perform the real interactions against the mock server</param>
        /// <exception cref="PactFailureException">Failed to verify the interactions</exception>
        public Task VerifyAsync(Func<IConsumerContext, Task> interact)
        {
            throw new NotImplementedException();
        }
    }
}
