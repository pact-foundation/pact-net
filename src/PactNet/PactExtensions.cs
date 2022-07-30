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
            NativeDriver server = new NativeDriver();
            PactHandle handle = InitialisePact(server, pact, PactSpecification.V2);

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
        public static IPactBuilderV3 WithHttpInteractions(this IPactV3 pact, int? port = null, IPAddress host = IPAddress.Loopback)
        {
            NativeDriver driver = new NativeDriver();
            PactHandle handle = InitialisePact(driver, pact, PactSpecification.V3);

            var builder = new PactBuilder(driver, handle, pact.Config, port, host);
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
            NativeDriver driver = new NativeDriver();
            PactHandle handle = InitialisePact(driver, pact, PactSpecification.V3);

            var builder = new MessagePactBuilder(driver, handle, pact.Config);
            return builder;
        }

        /// <summary>
        /// Initialise a new pact on the server with the correct version
        /// </summary>
        /// <param name="pact">Pact details</param>
        /// <param name="driver">Server</param>
        /// <param name="version">Spec version</param>
        /// <returns>Initialised pact handle</returns>
        private static PactHandle InitialisePact(INewPactDriver driver, IPact pact, PactSpecification version)
        {
            // TODO: Create the driver inside here and return IInteractionDriver instead, with the handle already wrapped inside

            InitialiseLogging(pact.Config.LogLevel);

            PactHandle handle = driver.NewPact(pact.Consumer, pact.Provider);
            driver.WithSpecification(handle, version);
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
