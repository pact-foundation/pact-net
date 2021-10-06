namespace PactNet.AspNetCore.Messaging
{
    /// <summary>
    /// DTO for the request body sent by the pact verifier
    /// </summary>
    internal class MessageInteraction
    {
        /// <summary>
        /// Message description
        /// </summary>
        public string Description { get; set; }
    }
}
