namespace PactNet
{
    /// <summary>
    /// Pact Builder
    /// </summary>
    public interface IPactBuilder
    {
        /// <summary>
        /// Finalise the pact
        /// </summary>
        /// <returns>Pact context in which to run interactions</returns>
        IPactContext Build();
    }

    /// <summary>
    /// Pact v2 Builder
    /// </summary>
    public interface IPactBuilderV2 : IPactBuilder
    {
        /// <summary>
        /// Add a new interaction to the pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 UponReceiving(string description);
    }

    /// <summary>
    /// Pact v3 Builder
    /// </summary>
    public interface IPactBuilderV3 : IPactBuilder
    {
        /// <summary>
        /// Add a new interaction to the pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 UponReceiving(string description);
    }
}
