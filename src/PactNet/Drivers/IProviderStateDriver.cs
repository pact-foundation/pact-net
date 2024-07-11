namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for setting provider states on an interaction
    /// </summary>
    public interface IProviderStateDriver
    {
        /// <summary>
        /// Add a provider state to the interaction
        /// </summary>
        /// <param name="description">Provider state description</param>
        void Given(string description);

        /// <summary>
        /// Add a provider state with a parameter to the interaction
        /// </summary>
        /// <param name="description">Provider state description</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        void GivenWithParam(string description, string name, string value);
    }
}
