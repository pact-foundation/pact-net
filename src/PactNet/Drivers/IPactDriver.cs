using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for creating a new pact and 
    /// </summary>
    internal interface IPactDriver
    {
        /// <summary>
        /// Create a new HTTP pact
        /// </summary>
        /// <param name="consumerName">Consumer name</param>
        /// <param name="providerName">Provider name</param>
        /// <param name="version">Specification version</param>
        /// <returns>HTTP pact driver</returns>
        IHttpPactDriver NewHttpPact(string consumerName, string providerName, PactSpecification version);

        /// <summary>
        /// Create a new message pact
        /// </summary>
        /// <param name="consumerName">Consumer name</param>
        /// <param name="providerName">Provider name</param>
        /// <param name="version">Specification version</param>
        /// <returns>Message pact driver driver</returns>
        IMessagePactDriver NewMessagePact(string consumerName, string providerName, PactSpecification version);

        /// <summary>
        /// Get the driver logs
        /// </summary>
        /// <returns>Logs</returns>
        string DriverLogs();
    }
}
