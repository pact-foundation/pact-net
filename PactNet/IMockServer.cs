namespace PactNet
{
    /// <summary>
    /// Mock server
    /// </summary>
    internal interface IMockServer
    {
        /// <summary>
        /// Start a new pact
        /// </summary>
        /// <returns>Interaction builder for the new pact</returns>
        IInteractionBuilder CreatePact();

        /// <summary>
        /// Write the pact file after all interactions are complete
        /// </summary>
        void WritePactFile();
    }
}