using Newtonsoft.Json;

namespace PactNet.AspNetCore.Messaging
{
    /// <summary>
    /// Defines the message middleware options
    /// </summary>
    public class MessagingVerifierOptions
    {
        /// <summary>
        /// The base path of the message route
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// Options for JSOn serialisation
        /// </summary>
        public JsonSerializerSettings DefaultJsonSettings { get; set; }
    }
}
