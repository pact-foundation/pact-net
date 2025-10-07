namespace PactNet.Drivers.Plugins
{
    /// <summary>
    /// Driver for plugin-based pacts
    /// </summary>
    internal interface IPluginPactDriver : ICompletedPactDriver
    {
        /// <summary>
        /// Create a new sync interaction on the current pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Interaction driver</returns>
        IPluginInteractionDriver NewSyncInteraction(string description);
    }
}
