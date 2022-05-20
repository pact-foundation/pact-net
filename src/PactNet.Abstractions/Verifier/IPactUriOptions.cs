namespace PactNet.Verifier
{
    /// <summary>
    /// Options for URI pact sources
    /// </summary>
    public interface IPactUriOptions
    {
        /// <summary>
        /// Use Basic authentication to access the URI
        /// </summary>
        /// <param name="username">Pact broker username</param>
        /// <param name="password">Pact broker password</param>
        /// <returns>Fluent builder</returns>
        IPactUriOptions BasicAuthentication(string username, string password);

        /// <summary>
        /// Use Token authentication to access the URI
        /// </summary>
        /// <param name="token">Auth token</param>
        /// <returns>Fluent builder</returns>
        IPactUriOptions TokenAuthentication(string token);
    }
}
