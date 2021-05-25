using System;
using System.Runtime.InteropServices;

namespace PactNet.Backend.Native
{
    /// <summary>
    /// Interaction builder
    /// </summary>
    public class NativeInteractionBuilder : IInteractionBuilder
    {
        private readonly PactHandle pact;
        private readonly int port;
        private readonly PactConfig config;

        private InteractionHandle interaction;

        /// <summary>
        /// URI for the mock provider
        /// </summary>
        public Uri MockProviderUri { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="NativeInteractionBuilder"/> class.
        /// </summary>
        /// <param name="pact">Parent pact handle</param>
        /// <param name="port">Port for the mock provider</param>
        /// <param name="config">Pact builder config</param>
        internal NativeInteractionBuilder(PactHandle pact, int port, PactConfig config)
        {
            this.pact = pact;
            this.port = port;
            this.config = config;

            this.MockProviderUri = new Uri($"http://localhost:{port}");
        }

        /// <summary>
        /// Start a new interaction
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder UponReceiving(string description)
        {
            this.interaction = MockServerInterop.NewInteraction(this.pact, description);

            // TODO: Is this actually necessary or does the above call already do this?
            MockServerInterop.UponReceiving(this.interaction, description);

            var requestBuilder = new NativeRequestBuilder(this.interaction, this.config.DefaultJsonSettings);
            return requestBuilder;
        }

        /// <summary>
        /// Verifies all configured interactions
        /// </summary>
        /// <exception cref="PactFailureException">Verification failed</exception>
        public void Verify()
        {
            IntPtr errorPtr = MockServerInterop.MockServerMismatches(this.port);
            string errors = string.Empty;

            if (errorPtr != IntPtr.Zero)
            {
                errors = Marshal.PtrToStringAnsi(errorPtr);
            }

            if (string.IsNullOrWhiteSpace(errors))
            {
                return;
            }

            this.config.WriteLine("Verification mismatches:");
            this.config.WriteLine(string.Empty);
            this.config.WriteLine(errors);

            throw new PactFailureException("Pact verification failed. See output for details");
        }
    }
}