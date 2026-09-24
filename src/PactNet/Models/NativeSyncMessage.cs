namespace PactNet.Models
{
    /// <summary>
    /// Placeholder for a native synchronous (request/response) message from the backend
    /// </summary>
    internal class NativeSyncMessage
    {
        /// <summary>
        /// The request message
        /// </summary>
        public NativeMessage Request { get; set; }

        /// <summary>
        /// The response messages
        /// </summary>
        public NativeMessage[] Response { get; set; }
    }
}
