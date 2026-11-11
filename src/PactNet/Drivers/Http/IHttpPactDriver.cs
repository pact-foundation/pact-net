using System;

namespace PactNet.Drivers.Http
{
    /// <summary>
    /// Driver for synchronous HTTP pacts
    /// </summary>
    internal interface IHttpPactDriver : ICompletedPactDriver
    {
        /// <summary>
        /// Create a new interaction on the current pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>HTTP interaction handle</returns>
        IHttpInteractionDriver NewHttpInteraction(string description);
    }
}
