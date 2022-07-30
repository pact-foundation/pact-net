using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for creating new interactions in a pact file
    /// </summary>
    internal interface IInteractionDriver
    {
        /// <summary>
        /// Create a new interaction on the given pact
        /// </summary>
        /// <param name="pact">Pact handle</param>
        /// <param name="description">Interaction description</param>
        /// <returns>Interaction handle</returns>
        InteractionHandle NewHttpInteraction(PactHandle pact, string description);

        /// <summary>
        /// Create a new message interaction on the given pact
        /// </summary>
        /// <param name="pact">Pact handle</param>
        /// <param name="description">Interaction description</param>
        /// <returns>Interaction handle</returns>
        InteractionHandle NewMessageInteraction(PactHandle pact, string description);

        /// <summary>
        /// Add a provider state to the interaction
        /// </summary>
        /// <param name="interaction">Interaction</param>
        /// <param name="description">Provider state description</param>
        /// <returns>Success</returns>
        bool Given(InteractionHandle interaction, string description);

        /// <summary>
        /// Add a provider state with a parameter to the interaction
        /// </summary>
        /// <param name="interaction">Interaction</param>
        /// <param name="description">Provider state description</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <returns>Success</returns>
        bool GivenWithParam(InteractionHandle interaction, string description, string name, string value);
    }
}
