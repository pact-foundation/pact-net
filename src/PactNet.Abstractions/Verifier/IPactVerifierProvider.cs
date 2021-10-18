namespace PactNet.Verifier
{
    /// <summary>
    /// Pact verifier provider
    /// </summary>
    public interface IPactVerifierProvider
    {
        /// <summary>
        /// Set the consumer name
        /// </summary>
        /// <param name="consumerName">Consumer name</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierConsumer HonoursPactWith(string consumerName);
    }
}
