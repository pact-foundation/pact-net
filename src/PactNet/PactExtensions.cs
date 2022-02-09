using System;
using PactNet.Interop;
using PactNet.Models;

namespace PactNet
{
    /// <summary>
    /// Extensions for <see cref="Pact"/>
    /// </summary>
    public static class PactExtensions
    {
        private static readonly object LogLocker = new object();
        private static bool LogInitialised = false;

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
        public static IPactBuilderV2 UsingNativeBackend(this IPactV2 pact, int? port = null, IPAddress host = IPAddress.Loopback)
        {
            MockServer server = new MockServer();
            PactHandle handle = InitialiseServer(server, pact, PactSpecification.V2);

            var builder = new PactBuilder(server, handle, pact.Config, port, host);
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
        public static IPactBuilderV3 UsingNativeBackend(this IPactV3 pact, int? port = null, IPAddress host = IPAddress.Loopback)
        {
            MockServer server = new MockServer();
            PactHandle handle = InitialiseServer(server, pact, PactSpecification.V3);

            var builder = new PactBuilder(server, handle, pact.Config, port, host);
            return builder;
        }

        /// <summary>
        /// Establish a new message pact using the native backend
        /// </summary>
        /// <param name="messagePact">Message Pact details</param>
        /// <returns>Pact builder</returns>
        public static IMessagePactBuilderV3 UsingNativeBackend(this IMessagePactV3 messagePact)
        {
            MockServer server = new MockServer();
            MessagePactHandle handle = InitialiseMessage(server, messagePact, PactSpecification.V3);

            var builder = new MessagePactBuilder(server, handle, messagePact.Config);
            return builder;
        }

        /// <summary>
        /// Initialise a new pact on the server with the correct version
        /// </summary>
        /// <param name="pact">Pact details</param>
        /// <param name="server">Server</param>
        /// <param name="version">Spec version</param>
        /// <returns>Initialised pact handle</returns>
        private static PactHandle InitialiseServer(MockServer server, IPact pact, PactSpecification version)
        {
            InitialiseLogging(pact.Config.LogLevel);

            PactHandle handle = server.NewPact(pact.Consumer, pact.Provider);
            server.WithSpecification(handle, version);
            return handle;
        }

        /// <summary>
        /// Initialise a new message pact with the correct version
        /// </summary>
        /// <param name="messagePact">Message Pact details</param>
        /// <param name="server">Server</param>
        /// <param name="version">Spec version</param>
        /// <returns>Initialised message pact handle</returns>
        private static MessagePactHandle InitialiseMessage(MockServer server, IMessagePact messagePact, PactSpecification version)
        {
            InitialiseLogging(messagePact.Config.LogLevel);

            MessagePactHandle handle = server.NewMessagePact(messagePact.Consumer, messagePact.Provider);

            return handle;
        }

        /// <summary>
        /// Initialise logging in the native library
        /// </summary>
        /// <param name="level">Log level</param>
        /// <exception cref="ArgumentOutOfRangeException">Invalid log level</exception>
        /// <remarks>Logging can only be initialised **once**. Subsequent calls will have no effect</remarks>
        private static void InitialiseLogging(PactLogLevel level)
        {
            lock (LogLocker)
            {
                if (LogInitialised)
                {
                    return;
                }

                NativeInterop.LogToBuffer(level switch
                {
                    PactLogLevel.Trace => LevelFilter.Trace,
                    PactLogLevel.Debug => LevelFilter.Debug,
                    PactLogLevel.Information => LevelFilter.Info,
                    PactLogLevel.Warn => LevelFilter.Warn,
                    PactLogLevel.Error => LevelFilter.Error,
                    PactLogLevel.None => LevelFilter.Off,
                    _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Invalid log level")
                });

                LogInitialised = true;
            }
        }
    }
}
