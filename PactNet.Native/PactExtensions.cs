using PactNet.Models;

namespace PactNet.Native
{
    /// <summary>
    /// Extensions for <see cref="Pact"/>
    /// </summary>
    public static class PactExtensions
    {
        /// <summary>
        /// Establish a new pact using the native backend
        /// </summary>
        /// <param name="pact">Pact details</param>
        /// <param name="port">Port for the mock server. If null, one will be assigned automatically</param>
        /// <param name="host">Host for the mock server</param>
        /// <returns>Pact builder</returns>
        /// <remarks>
        /// If multiple mock servers are started at the same time, you must make sure you don't supply the same port twice.
        /// It is advised that the port is not specified whenever possible to allow PactNet to allocate a port dynamically
        /// and ensure there are no port clashes
        /// </remarks>
        public static IPactBuilder UsingNativeBackend(this IPact pact, int? port = null, IPAddress host = IPAddress.Loopback)
        {
            var handle = MockServerInterop.NewPact(pact.Consumer, pact.Provider);

            PactSpecification specification = pact.SpecificationVersion switch
            {
                "1.0.0" => PactSpecification.V1,
                "1.1.0" => PactSpecification.V1_1,
                "2.0.0" => PactSpecification.V2,
                "3.0.0" => PactSpecification.V3,
                "4.0.0" => PactSpecification.V4,
                _ => PactSpecification.Unknown
            };

            MockServerInterop.WithSpecification(handle, specification);

            var builder = new NativePactBuilder(handle, pact.Config, port, host);
            return builder;
        }
    }
}