using PactNet.Models;

namespace PactNet.Native
{
    /// <summary>
    /// Extensions for <see cref="Pact"/>
    /// </summary>
    public static class PactExtensions
    {
        /// <summary>
        /// Establish a new messagePact using the native backend
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
        public static IPactBuilderV2 UsingNativeBackend(this IPactV2 pact, int? port = null, IPAddress host = IPAddress.Loopback)
        {
            NativeMockServer server = new NativeMockServer();
            PactHandle handle = InitialiseServer(server, pact, PactSpecification.V2);

            var builder = new NativePactBuilder(server, handle, pact.Config, port, host);
            return builder;
        }

        /// <summary>
        /// Establish a new messagePact using the native backend
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
        public static IPactBuilderV3 UsingNativeBackend(this IPactV3 pact, int? port = null, IPAddress host = IPAddress.Loopback)
        {
            NativeMockServer server = new NativeMockServer();
            PactHandle handle = InitialiseServer(server, pact, PactSpecification.V3);

            var builder = new NativePactBuilder(server, handle, pact.Config, port, host);
            return builder;
        }

        /// <summary>
        /// Establish a new messagePact using the native backend
        /// </summary>
        /// <param name="messagePact">Pact details</param>
        /// <returns>Pact builder</returns>
        /// <remarks>
        /// If multiple mock servers are started at the same time, you must make sure you don't supply the same port twice.
        /// It is advised that the port is not specified whenever possible to allow PactNet to allocate a port dynamically
        /// and ensure there are no port clashes
        /// </remarks>
        public static IMessagePactBuilderV3 UsingNativeBackend(this IMessagePactV3 messagePact)
        {
            NativeMockServer server = new NativeMockServer();
            MessagePactHandle handle = InitialiseMessage(server, messagePact, PactSpecification.V3);

            var builder = new NativeMessagePactBuilder(server, handle, messagePact.Config);
            return builder;
        }

        /// <summary>
        /// Initialise a new messagePact on the server with the correct version
        /// </summary>
        /// <param name="pact">Pact details</param>
        /// <param name="server">Server</param>
        /// <param name="version">Spec version</param>
        /// <returns>Initialised messagePact handle</returns>
        private static PactHandle InitialiseServer(NativeMockServer server, IPact pact, PactSpecification version)
        {
            PactHandle handle = server.NewPact(pact.Consumer, pact.Provider);
            server.WithSpecification(handle, version);
            return handle;
        }

        /// <summary>
        /// Initialise a new message messagePact with the correct version
        /// </summary>
        /// <param name="messagePact">Pact details</param>
        /// <param name="server">Server</param>
        /// <param name="version">Spec version</param>
        /// <returns>Initialised message messagePact handle</returns>
        private static MessagePactHandle InitialiseMessage(NativeMockServer server, IMessagePact messagePact, PactSpecification version)
        {
            PactHandle handle = server.NewPact(messagePact.Consumer, messagePact.Provider);
            server.WithSpecification(handle, version);

            return server.NewMessagePact(messagePact.Consumer, messagePact.Provider);
        }
    }
}
