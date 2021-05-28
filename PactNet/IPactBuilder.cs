namespace PactNet
{
    /// <summary>
    /// Pact Builder
    /// </summary>
    public interface IPactBuilder
    {
        /// <summary>
        /// Add a new interaction to the pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilder UponReceiving(string description);

        /// <summary>
        /// Finalise the pact
        /// </summary>
        /// <returns>Pact context in which to run interactions</returns>
        IPactContext Build();
    }
}