using System;
using PactNet.Interop;

namespace PactNet.Drivers.Http
{
    /// <summary>
    /// Driver for synchronous HTTP pacts
    /// </summary>
    internal class HttpPactDriver : AbstractPactDriver, IHttpPactDriver
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="HttpPactDriver"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        internal HttpPactDriver(PactHandle pact) : base(pact)
        {
        }

        /// <summary>
        /// Create a new interaction on the current pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>HTTP interaction handle</returns>
        public IHttpInteractionDriver NewHttpInteraction(string description)
        {
            InteractionHandle interaction = NativeInterop.NewInteraction(this.pact, description);
            return new HttpInteractionDriver(interaction);
        }
    }
}
