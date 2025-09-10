using PactNet.Drivers;
using PactNet.Interop;
using PactNet.Models;

namespace PactNet
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
        public static IPactBuilderV2 WithHttpInteractions(this IPactV2 pact, int? port = null, IPAddress host = IPAddress.Loopback)
        {
            pact.Config.LogLevel.InitialiseLogging();

            IPactDriver driver = new PactDriver();
            IHttpPactDriver httpPact = driver.NewHttpPact(pact.Consumer, pact.Provider, PactSpecification.V2);

            var builder = new PactBuilder(httpPact, pact.Config, port, host);
            return builder;
        }

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
        public static IPactBuilderV3 WithHttpInteractions(this IPactV3 pact, int? port = null, IPAddress host = IPAddress.Loopback)
        {
            pact.Config.LogLevel.InitialiseLogging();

            IPactDriver driver = new PactDriver();
            IHttpPactDriver httpPact = driver.NewHttpPact(pact.Consumer, pact.Provider, PactSpecification.V3);

            var builder = new PactBuilder(httpPact, pact.Config, port, host);
            return builder;
        }

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
        public static IPactBuilderV4 WithHttpInteractions(this IPactV4 pact, int? port = null, IPAddress host = IPAddress.Loopback)
        {
            pact.Config.LogLevel.InitialiseLogging();

            IPactDriver driver = new PactDriver();
            IHttpPactDriver httpPact = driver.NewHttpPact(pact.Consumer, pact.Provider, PactSpecification.V4);

            var builder = new PactBuilder(httpPact, pact.Config, port, host);
            return builder;
        }

        /// <summary>
        /// Add asynchronous message (i.e. consumer/producer) interactions to the pact
        /// </summary>
        /// <param name="pact">Pact details</param>
        /// <returns>Pact builder</returns>
        public static IMessagePactBuilderV3 WithMessageInteractions(this IPactV3 pact)
        {
            pact.Config.LogLevel.InitialiseLogging();

            IPactDriver driver = new PactDriver();
            IMessagePactDriver messagePact = driver.NewMessagePact(pact.Consumer, pact.Provider, PactSpecification.V3);

            var builder = new MessagePactBuilder(messagePact, pact.Config, PactSpecification.V3);
            return builder;
        }

        /// <summary>
        /// Add asynchronous message (i.e. consumer/producer) interactions to the pact
        /// </summary>
        /// <param name="pact">Pact details</param>
        /// <returns>Pact builder</returns>
        public static IMessagePactBuilderV4 WithMessageInteractions(this IPactV4 pact)
        {
            pact.Config.LogLevel.InitialiseLogging();

            IPactDriver driver = new PactDriver();
            IMessagePactDriver messagePact = driver.NewMessagePact(pact.Consumer, pact.Provider, PactSpecification.V4);

            var builder = new MessagePactBuilder(messagePact, pact.Config, PactSpecification.V4);
            return builder;
        }
    }
}
