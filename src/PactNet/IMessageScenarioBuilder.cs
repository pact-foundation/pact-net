namespace PactNet
{
    /// <summary>
    /// Defines the message scenario builder interface
    /// </summary>
    public interface IMessageScenarioBuilder
    {
        /// <summary>
        /// Set the description of the scenario
        /// </summary>
        /// <param name="description">the name of the interaction</param>
        /// <returns>The content message fluent builder</returns>
        IMessageScenarioContentBuilder WhenReceiving(string description);
    }
}
