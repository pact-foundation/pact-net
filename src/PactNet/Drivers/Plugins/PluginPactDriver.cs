using System;
using PactNet.Interop;

namespace PactNet.Drivers.Plugins
{
    /// <summary>
    /// Driver for plugin-based pacts
    /// </summary>
    internal class PluginPactDriver : AbstractPactDriver, IPluginPactDriver, IDisposable
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


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~PluginPactDriver()
        {
            this.ReleaseUnmanagedResources();
        }

        /// <summary>
        /// Release unmanaged resources
        /// </summary>
        private void ReleaseUnmanagedResources()
        {
            NativeInterop.CleanupPlugins(pact);
        }
    }
}
