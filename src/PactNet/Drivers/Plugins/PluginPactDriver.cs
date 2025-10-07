using PactNet.Interop;

namespace PactNet.Drivers.Plugins
{
    /// <summary>
    /// Driver for plugin-based pacts
    /// </summary>
    internal class PluginPactDriver : AbstractPactDriver, IPluginPactDriver
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="PluginPactDriver"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        internal PluginPactDriver(PactHandle pact) : base(pact)
        {
        }

        /// <summary>
        /// Create a new sync interaction on the current pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Interaction driver</returns>
        public IPluginInteractionDriver NewSyncInteraction(string description)
        {
            InteractionHandle interaction = NativeInterop.NewSyncMessageInteraction(this.pact, description);
            return new PluginInteractionDriver(interaction);
        }
    }
}
