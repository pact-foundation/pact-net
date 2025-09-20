using PactNet.Drivers.Http;
using PactNet.Drivers.Message;
using PactNet.Drivers.Plugins;
using PactNet.Exceptions;
using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for creating new pacts
    /// </summary>
    internal class PactDriver : IPactDriver
    {
        /// <summary>
        /// Create a new HTTP pact
        /// </summary>
        /// <param name="consumerName">Consumer name</param>
        /// <param name="providerName">Provider name</param>
        /// <param name="version">Specification version</param>
        /// <returns>HTTP pact driver</returns>
        public IHttpPactDriver NewHttpPact(string consumerName, string providerName, PactSpecification version)
        {
            PactHandle pact = CreatePactHandle(consumerName, providerName, version);
            return new HttpPactDriver(pact);
        }

        /// <summary>
        /// Create a new message pact
        /// </summary>
        /// <param name="consumerName">Consumer name</param>
        /// <param name="providerName">Provider name</param>
        /// <param name="version">Specification version</param>
        /// <returns>Message pact driver</returns>
        public IMessagePactDriver NewMessagePact(string consumerName, string providerName, PactSpecification version)
        {
            PactHandle pact = CreatePactHandle(consumerName, providerName, version);
            return new MessagePactDriver(pact);
        }

        /// <summary>
        /// Create a new plugin pact
        /// </summary>
        /// <param name="consumerName">Consumer name</param>
        /// <param name="providerName">Provider name</param>
        /// <param name="pluginName">Plugin name</param>
        /// <param name="pluginVersion">Plugin version</param>
        /// <param name="version">Specification version</param>
        /// <returns>Plugin pact driver</returns>
        public IPluginPactDriver NewPluginPact(string consumerName, string providerName, string pluginName, string pluginVersion, PactSpecification version)
        {
            PactHandle pact = NativeInterop.NewPact(consumerName, providerName);
            NativeInterop.WithSpecification(pact, version).CheckInteropSuccess();

            uint code = NativeInterop.UsingPlugin(pact, pluginName, pluginVersion);

            if (code == 0)
            {
                return new PluginPactDriver(pact);
            }

            throw code switch
            {
                1 => new PactFailureException("Unable to setup the plugin - general panic"),
                2 => new PactFailureException("Unable to setup the plugin - invalid plugin name or version"),
                3 => new PactFailureException("Unable to setup the plugin - invalid pact handle"),
                _ => new PactFailureException($"Unable to setup the plugin - unknown error {code}")
            };
        }

        /// <summary>
        /// Create a new pact handle and set the specification version
        /// </summary>
        /// <param name="consumerName">Consumer name</param>
        /// <param name="providerName">Provider name</param>
        /// <param name="version">Specification version</param>
        /// <returns>Pact handle</returns>
        private static PactHandle CreatePactHandle(string consumerName, string providerName, PactSpecification version)
        {
            PactHandle pact = NativeInterop.NewPact(consumerName, providerName);
            NativeInterop.WithSpecification(pact, version).CheckInteropSuccess();
            return pact;
        }

        /// <summary>
        /// Get the driver logs
        /// </summary>
        /// <returns>Logs</returns>
        public string DriverLogs() => NativeInterop.FetchLogBuffer(null);
    }
}
