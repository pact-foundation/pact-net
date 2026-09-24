using System;
using System.Runtime.InteropServices;
using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for synchronous (request/response) message interactions
    /// </summary>
    internal class SyncMessageInteractionDriver : AbstractPactDriver, ISyncMessageInteractionDriver
    {
        private readonly InteractionHandle interaction;

        /// <summary>
        /// Initialises a new instance of the <see cref="SyncMessageInteractionDriver"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        /// <param name="interaction">Interaction handle</param>
        internal SyncMessageInteractionDriver(PactHandle pact, InteractionHandle interaction) : base(pact)
        {
            this.interaction = interaction;
        }

        /// <summary>
        /// Add a provider state to the interaction
        /// </summary>
        /// <param name="description">Provider state description</param>
        public void Given(string description)
            => NativeInterop.Given(this.interaction, description).CheckInteropSuccess();

        /// <summary>
        /// Add a provider state with a parameter to the interaction
        /// </summary>
        /// <param name="description">Provider state description</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        public void GivenWithParam(string description, string name, string value)
            => NativeInterop.GivenWithParam(this.interaction, description, name, value).CheckInteropSuccess();

        /// <summary>
        /// Set the metadata of the request message
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        public void WithRequestMetadata(string key, string value)
            => NativeInterop.WithMetadata(this.interaction, key, value, InteractionPart.Request).CheckInteropSuccess();

        /// <summary>
        /// Set the metadata of the response message
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        public void WithResponseMetadata(string key, string value)
            => NativeInterop.WithMetadata(this.interaction, key, value, InteractionPart.Response).CheckInteropSuccess();

        /// <summary>
        /// Set the contents of the request message
        /// </summary>
        /// <param name="contentType">the content type</param>
        /// <param name="body">the body of the request message</param>
        public void WithRequestContents(string contentType, string body)
            => NativeInterop.WithBody(this.interaction, InteractionPart.Request, contentType, body).CheckInteropSuccess();

        /// <summary>
        /// Set the contents of the response message
        /// </summary>
        /// <param name="contentType">the content type</param>
        /// <param name="body">the body of the response message</param>
        public void WithResponseContents(string contentType, string body)
            => NativeInterop.WithBody(this.interaction, InteractionPart.Response, contentType, body).CheckInteropSuccess();

        /// <summary>
        /// Get the actual request and response contents, with any matchers removed and any
        /// configured generators applied
        /// </summary>
        /// <returns>The generated request and response contents</returns>
        public string GenerateContents()
        {
            IntPtr pointer = NativeInterop.SyncMessageGenerateContents(this.interaction);
            string body = Marshal.PtrToStringAnsi(pointer);
            return body;
        }
    }
}
