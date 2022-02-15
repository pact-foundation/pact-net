namespace PactNet.Verifier.Messaging
{
    /// <summary>
    /// DTO for the request body sent by the pact verifier
    /// </summary>
    public class MessageInteraction
    {
        /// <summary>
        /// Message description
        /// </summary>
        public string Description { get; set; }
    }
}
