using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PactNet.Drivers;
using PactNet.Exceptions;
using PactNet.Models;

namespace PactNet
{
    /// <summary>
    /// Verifies a configured message interaction
    /// </summary>
    internal class ConfiguredMessageVerifier : IConfiguredMessageVerifier
    {
        // the native message returned from the FFI always uses camel case property
        // names, but the inner content may use different settings supplied by the user
        private static readonly JsonSerializerSettings NativeMessageSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly IMessageInteractionDriver driver;
        private readonly PactConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="ConfiguredMessageVerifier"/>
        /// </summary>
        /// <param name="driver">Pact driver</param>
        /// <param name="config">Pact configuration</param>
        internal ConfiguredMessageVerifier(IMessageInteractionDriver driver, PactConfig config)
        {
            this.driver = driver;
            this.config = config;
        }

        /// <summary>
        /// Verify a message is read and handled correctly and write the message pact
        /// </summary>
        /// <param name="handler">The method using the message</param>
        public void Verify<T>(Action<T> handler)
        {
            try
            {
                var messageReified = this.MessageReified<T>();

                handler(messageReified);

                this.driver.WritePactFile(this.config.PactDir);
            }
            catch (Exception e)
            {
                throw new PactMessageConsumerVerificationException($"The message could not be verified by the consumer handler", e);
            }
        }

        /// <summary>
        /// Verify a message is read and handled correctly and write the message pact
        /// </summary>
        /// <param name="handler">The method using the message</param>
        public async Task VerifyAsync<T>(Func<T, Task> handler)
        {
            try
            {
                var messageReified = this.MessageReified<T>();

                await handler(messageReified);

                this.driver.WritePactFile(this.config.PactDir);
            }
            catch (Exception e)
            {
                throw new PactMessageConsumerVerificationException($"The message could not be verified by the consumer handler", e);
            }
        }

        /// <summary>
        /// Try to read the reified message
        /// </summary>
        /// <typeparam name="T">the type of message</typeparam>
        /// <returns>the message</returns>
        private T MessageReified<T>()
        {
            string reified = this.driver.Reify();
            NativeMessage content = JsonConvert.DeserializeObject<NativeMessage>(reified, NativeMessageSettings);

            string contentString = ((JToken)content.Contents).ToString(Formatting.None);
            T messageReified = JsonConvert.DeserializeObject<T>(contentString, this.config.DefaultJsonSettings);

            return messageReified;
        }
    }
}
