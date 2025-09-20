﻿using System;
using System.Runtime.InteropServices;
using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for asynchronous message interactions
    /// </summary>
    internal class MessageInteractionDriver : IMessageInteractionDriver
    {
        private readonly PactHandle pact;
        private readonly InteractionHandle interaction;

        /// <summary>
        /// Initialises a new instance of the <see cref="MessageInteractionDriver"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        /// <param name="interaction">Interaction handle</param>
        internal MessageInteractionDriver(PactHandle pact, InteractionHandle interaction)
        {
            this.pact = pact;
            this.interaction = interaction;
        }

        /// <summary>
        /// Add a provider state to the interaction
        /// </summary>
        /// <param name="description">Provider state description</param>
        public void Given(string description)
            => PactInterop.Given(this.interaction, description).ThrowExceptionOnFailure();

        /// <summary>
        /// Add a provider state with a parameter to the interaction
        /// </summary>
        /// <param name="description">Provider state description</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        public void GivenWithParam(string description, string name, string value)
            => PactInterop.GivenWithParam(this.interaction, description, name, value).ThrowExceptionOnFailure();

        /// <summary>
        /// Set the description of the message interaction
        /// </summary>
        /// <param name="description">message description</param>
        public void ExpectsToReceive(string description)
            => MessagingInterop.MessageExpectsToReceive(this.interaction, description);

        /// <summary>
        /// Set the metadata of the message
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        public void WithMetadata(string key, string value)
            => MessagingInterop.MessageWithMetadata(this.interaction, key, value);

        /// <summary>
        /// Set the contents of the message
        /// </summary>
        /// <param name="contentType">the content type</param>
        /// <param name="body">the body of the message</param>
        /// <param name="size">the size of the message</param>
        public void WithContents(string contentType, string body, uint size)
            => MessagingInterop.MessageWithContents(this.interaction, contentType, body, new UIntPtr(0));

        /// <summary>
        /// Returns the message without the matchers
        /// </summary>
        /// <returns>Reified message</returns>
        public string Reify()
        {
            IntPtr pointer = MessagingInterop.MessageReify(this.interaction);
            string body = Marshal.PtrToStringAnsi(pointer);
            return body;
        }

        public void WritePactFile(string directory) => PactFileWriter.WritePactFile(this.pact, directory);
    }
}
