using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PactNet.Drivers;
using PactNet.Exceptions;
using PactNet.Interop;
using PactNet.Models;

namespace PactNet
{
    /// <summary>
    /// Verifies a configured synchronous (request/response) message interaction
    /// </summary>
    internal class ConfiguredSyncMessageVerifier : IConfiguredSyncMessageVerifier
    {
        // the native message returned from the FFI always uses camel case property
        // names, but the inner content may use different settings supplied by the user
        private static readonly JsonSerializerOptions NativeMessageSettings = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly ISyncMessageInteractionDriver driver;
        private readonly PactConfig config;
        private readonly PactSpecification version;

        /// <summary>
        /// Initialises a new instance of the <see cref="ConfiguredSyncMessageVerifier"/>
        /// </summary>
        /// <param name="driver">Pact driver</param>
        /// <param name="config">Pact configuration</param>
        /// <param name="version">Pact specification version</param>
        internal ConfiguredSyncMessageVerifier(ISyncMessageInteractionDriver driver, PactConfig config, PactSpecification version)
        {
            this.driver = driver;
            this.config = config;
            this.version = version;
        }

        /// <summary>
        /// Verify a request message is read and handled correctly, and that the response returned
        /// by the handler matches the configured response content, then write the message pact
        /// </summary>
        /// <param name="handler">The method handling the request message and producing a response</param>
        public void Verify<TRequest, TResponse>(Func<TRequest, TResponse> handler)
        {
            try
            {
                NativeSyncMessage generated = this.GeneratedMessage();
                TRequest request = DeserializeContent<TRequest>((JsonElement)generated.Request.Contents);

                TResponse actualResponse = handler(request);

                this.VerifyResponse(actualResponse);
                this.driver.WritePactFile(this.config.PactDir);
            }
            catch (PactMessageConsumerVerificationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new PactMessageConsumerVerificationException($"The message could not be verified by the consumer handler", e);
            }
        }

        /// <summary>
        /// Verify a request message is read and handled correctly, and that the response returned
        /// by the handler matches the configured response content, then write the message pact
        /// </summary>
        /// <param name="handler">The method handling the request message and producing a response</param>
        public async Task VerifyAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
        {
            try
            {
                NativeSyncMessage generated = this.GeneratedMessage();
                TRequest request = DeserializeContent<TRequest>((JsonElement)generated.Request.Contents);

                TResponse actualResponse = await handler(request);

                this.VerifyResponse(actualResponse);
                this.driver.WritePactFile(this.config.PactDir);
            }
            catch (PactMessageConsumerVerificationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new PactMessageConsumerVerificationException($"The message could not be verified by the consumer handler", e);
            }
        }

        /// <summary>
        /// Get the generated message, with any matchers removed and any configured generators applied
        /// </summary>
        /// <returns>the native message</returns>
        private NativeSyncMessage GeneratedMessage()
        {
            string generated = this.driver.GenerateContents();
            return JsonSerializer.Deserialize<NativeSyncMessage>(generated, NativeMessageSettings);
        }

        /// <summary>
        /// Check the handler's response against the configured response, applying its matching rules
        /// </summary>
        /// <param name="actualResponse">the response returned by the handler under test</param>
        private void VerifyResponse<TResponse>(TResponse actualResponse)
        {
            string actualJson = JsonSerializer.Serialize(actualResponse, this.config.DefaultJsonSettings);
            string mismatchesJson = this.driver.MatchResponseContents(0, "application/json", actualJson);

            using JsonDocument mismatches = JsonDocument.Parse(mismatchesJson);

            if (mismatches.RootElement.GetArrayLength() > 0)
            {
                IEnumerable<string> descriptions = mismatches.RootElement
                                                             .EnumerateArray()
                                                             .Select(DescribeMismatch);

                throw new PactMessageConsumerVerificationException(
                    "The response returned by the consumer handler did not match the configured response:" +
                    Environment.NewLine + string.Join(Environment.NewLine, descriptions));
            }
        }

        /// <summary>
        /// Describe a mismatch returned by the FFI, e.g. "$.status: Expected 1 (Integer) to be the same type as 'shipped' (String)"
        /// </summary>
        private static string DescribeMismatch(JsonElement mismatch)
        {
            string description = mismatch.TryGetProperty("mismatch", out JsonElement text) ? text.GetString() : mismatch.GetRawText();

            return mismatch.TryGetProperty("path", out JsonElement path)
                       ? $"  {path.GetString()}: {description}"
                       : $"  {description}";
        }

        /// <summary>
        /// Deserialize the content of the request or response, with any matchers removed and any
        /// configured generators applied
        /// </summary>
        /// <typeparam name="T">the type to deserialize the content into</typeparam>
        /// <param name="contents">the generated message contents</param>
        /// <returns>the deserialized content</returns>
        private T DeserializeContent<T>(JsonElement contents)
        {
            // Synchronous messages only exist in the V4 Pact format, so the generated contents
            // always use the V4 body envelope (a `content` field)
            string contentString = contents.GetProperty("content").GetRawText();

            return JsonSerializer.Deserialize<T>(contentString, this.config.DefaultJsonSettings);
        }
    }
}
