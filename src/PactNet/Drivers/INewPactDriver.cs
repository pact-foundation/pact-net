using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for creating a new pact and 
    /// </summary>
    internal interface INewPactDriver
    {
        /// <summary>
        /// Create a new pact
        /// </summary>
        /// <param name="consumerName">Consumer name</param>
        /// <param name="providerName">Provider name</param>
        /// <returns>Pact handle</returns>
        PactHandle NewPact(string consumerName, string providerName);

        /// <summary>
        /// Set the pact specification version
        /// </summary>
        /// <param name="pact">Pact handle</param>
        /// <param name="version">Specification version</param>
        /// <returns>Success</returns>
        bool WithSpecification(PactHandle pact, PactSpecification version);
    }
}
