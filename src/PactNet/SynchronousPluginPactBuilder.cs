using PactNet.Drivers.Plugins;
using PactNet.Models;

namespace PactNet
{
    /// <summary>
    /// Synchronous plugin pact builder
    /// </summary>
    internal class SynchronousPluginPactBuilder : AbstractPactBuilder, ISynchronousPluginPactBuilderV4
    {
        private readonly IPluginPactDriver pact;

        /// <summary>
        /// Intialises a new instance of the <see cref="SynchronousPluginPactBuilder"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        /// <param name="config">Pact configuration</param>
        /// <param name="port">Mock server port</param>
        /// <param name="host">Mock server host</param>
        /// <param name="transport">Plugin transport</param>
        public SynchronousPluginPactBuilder(IPluginPactDriver pact,
                                            PactConfig config,
                                            int? port,
                                            IPAddress host,
                                            string transport = null)
            : base(pact, config, port, host, transport)
        {
            this.pact = pact;
        }

        /// <summary>
        /// Add a new interaction to the pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        public ISynchronousPluginRequestBuilderV4 UponReceiving(string description)
        {
            return new SynchronousPluginRequestBuilder(this.pact.NewSyncInteraction(description));
        }

        public void Dispose() => this.pact?.Dispose();
    }
}
