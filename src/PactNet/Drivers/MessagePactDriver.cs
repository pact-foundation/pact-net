using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for message pacts
    /// </summary>
    internal class MessagePactDriver : AbstractPactDriver, IMessagePactDriver
    {
        private readonly PactHandle pact;

        /// <summary>
        /// Initialises a new instance of the <see cref="MessagePactDriver"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        internal MessagePactDriver(PactHandle pact) : base(pact)
        {
            this.pact = pact;
        }

        /// <summary>
        /// Create a new message interaction on the current pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Message interaction driver</returns>
        public IMessageInteractionDriver NewMessageInteraction(string description)
        {
            InteractionHandle interaction = NativeInterop.NewMessageInteraction(this.pact, description);
            return new MessageInteractionDriver(this.pact, interaction);
        }

        /// <summary>
        /// Add metadata to the message pact
        /// </summary>
        /// <param name="namespace">the namespace</param>
        /// <param name="name">the name of the parameter</param>
        /// <param name="value">the value of the parameter</param>
        public void WithMessagePactMetadata(string @namespace, string name, string value)
            => NativeInterop.WithMessagePactMetadata(this.pact, @namespace, name, value).CheckInteropSuccess();
    }
}
