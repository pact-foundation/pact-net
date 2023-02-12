namespace PactNet.Verifier
{
    /// <summary>
    /// Options for controlling provider state calls
    /// </summary>
    public interface IProviderStateOptions
    {
        /// <summary>
        /// Provide request callbacks after each interaction is verified
        /// </summary>
        /// <returns>Fluent builder</returns>
        IProviderStateOptions WithTeardown();

        /// <summary>
        /// Configure the style in which the provider state endpoint is invoked
        /// </summary>
        /// <param name="style">Provider state style</param>
        /// <returns>Fluent builder</returns>
        IProviderStateOptions WithStyle(ProviderStateStyle style);
    }
}
