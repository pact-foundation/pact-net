using System;
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
        [Obsolete("Use WithHttpInteractions instead. Will be removed in PactNet 5.0.0")]
        public static IPactBuilderV2 UsingNativeBackend(this IPactV2 pact, int? port = null, IPAddress host = IPAddress.Loopback)
            => pact.WithHttpInteractions(port, host);

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
        [Obsolete("Use WithHttpInteractions instead. Will be removed in PactNet 5.0.0")]
        public static IPactBuilderV3 UsingNativeBackend(this IPactV3 pact, int? port = null, IPAddress host = IPAddress.Loopback)
            => pact.WithHttpInteractions(port, host);

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
            InitialiseLogging(pact.Config.LogLevel);

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
            InitialiseLogging(pact.Config.LogLevel);

            IPactDriver driver = new PactDriver();
            IHttpPactDriver httpPact = driver.NewHttpPact(pact.Consumer, pact.Provider, PactSpecification.V3);

            var builder = new PactBuilder(httpPact, pact.Config, port, host);
            return builder;
        }

        /// <summary>
        /// Establish a new message pact using the native backend
        /// </summary>
        /// <param name="messagePact">Message Pact details</param>
        /// <returns>Pact builder</returns>
        [Obsolete("Use WithMessageInteractions instead. Will be removed in PactNet 5.0.0")]
        public static IMessagePactBuilderV3 UsingNativeBackend(this IMessagePactV3 messagePact)
        {
            var pact = Pact.V3(messagePact.Consumer, messagePact.Provider, messagePact.Config);
            return pact.WithMessageInteractions();
        }

        /// <summary>
        /// Add asynchronous message (i.e. consumer/producer) interactions to the pact
        /// </summary>
        /// <param name="pact">Pact details</param>
        /// <returns>Pact builder</returns>
        public static IMessagePactBuilderV3 WithMessageInteractions(this IPactV3 pact)
        {
            InitialiseLogging(pact.Config.LogLevel);

            IPactDriver driver = new PactDriver();
            IMessagePactDriver messagePact = driver.NewMessagePact(pact.Consumer, pact.Provider, PactSpecification.V3);

            var builder = new MessagePactBuilder(messagePact, pact.Config);
            return builder;
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
