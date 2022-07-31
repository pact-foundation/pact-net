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
            PactHandle pact = NativeInterop.NewPact(consumerName, providerName);
            NativeInterop.WithSpecification(pact, version).CheckInteropSuccess();

            return new HttpPactDriver(pact);
        }

        /// <summary>
        /// Create a new message pact
        /// </summary>
        /// <param name="consumerName">Consumer name</param>
        /// <param name="providerName">Provider name</param>
        /// <param name="version">Specification version</param>
        /// <returns>Message pact driver driver</returns>
        public IMessagePactDriver NewMessagePact(string consumerName, string providerName, PactSpecification version)
        {
            PactHandle pact = NativeInterop.NewPact(consumerName, providerName);
            NativeInterop.WithSpecification(pact, version).CheckInteropSuccess();

            return new MessagePactDriver(pact);
        }

        /// <summary>
        /// Get the driver logs
        /// </summary>
        /// <returns>Logs</returns>
        public string DriverLogs() => NativeInterop.FetchLogBuffer(null);
    }
}
