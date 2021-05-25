using System;

namespace PactNet.Backend.Remote
{
    /// <summary>
    /// Remote mock server
    /// </summary>
    public class RemoteMockServer : IMockServer
    {
        private readonly string consumer;
        private readonly string provider;
        private readonly PactConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="RemoteMockServer"/> class.
        /// </summary>
        /// <param name="consumer">Consumer name</param>
        /// <param name="provider">Provider name</param>
        /// <param name="config">Mock server config</param>
        public RemoteMockServer(string consumer, string provider, PactConfig config)
        {
            this.consumer = consumer;
            this.provider = provider;
            this.config = config;
        }

        /// <summary>
        /// Start a new pact
        /// </summary>
        /// <returns>Interaction builder for the new pact</returns>
        public IInteractionBuilder CreatePact()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write the pact file after all interactions are complete
        /// </summary>
        public void WritePactFile()
        {
            throw new NotImplementedException();
        }
    }
}