using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PactNet.Verifier.Messaging;

namespace PactNet.AspNetCore.Messaging
{
    /// <summary>
    /// Defines the message middleware
    /// It handles the queries for the verifier when there are message interactions
    /// </summary>
    public class MessageMiddleware
    {
        /// <summary>
        /// The next pointer
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// The message middleware options
        /// </summary>
        private readonly MessagingVerifierOptions options;

        /// <summary>
        /// The provider state accessor
        /// </summary>
        private readonly IMessagingScenarioAccessor messagingScenarioAccessor;

        /// <summary>
        /// Creates an instance of <see cref="MessageMiddleware"/>
        /// </summary>
        /// <param name="options">the middleware options</param>
        /// <param name="next">the next handle</param>
        /// <param name="messagingScenarioAccessor">the messaging scenario accessor</param>
        public MessageMiddleware(IOptions<MessagingVerifierOptions> options, RequestDelegate next, IMessagingScenarioAccessor messagingScenarioAccessor)
        {
            this.next = next;
            this.messagingScenarioAccessor = messagingScenarioAccessor;
            this.options = options.Value;
        }

        /// <summary>
        /// The invoke method of the middleware
        /// </summary>
        /// <param name="context">The http context</param>
        /// <returns>A task</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (!(context.Request.Path.Value?.StartsWith(options.BasePath) ?? false))
            {
                await this.next(context);
                return;
            }

            string interactionDescription = await GetInteractionDescriptionAsync(context);

            if (string.IsNullOrWhiteSpace(interactionDescription))
            {
                await WriteErrorInResponseAsync(context, "The interaction is invalid for messaging handling.", HttpStatusCode.BadRequest);
                return;
            }

            IScenario scenario = messagingScenarioAccessor.GetByDescription(interactionDescription);
            dynamic response = scenario.InvokeScenario();

            if (response == null)
            {
                await WriteErrorInResponseAsync(context, "The scenario invocation returned a null object. You must setup your messaging scenario so it returns a non-null object.", HttpStatusCode.NotFound);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (scenario.Metadata != null)
            {
                AddMetadataToResponse(context, scenario.Metadata);
            }

            await WriteToResponseAsync(context, response);
        }

        /// <summary>
        /// Write a dynamic message to the http response
        /// </summary>
        /// <param name="context">the http context</param>
        /// <param name="response">the dynamic message</param>
        private static Task WriteToResponseAsync(HttpContext context, dynamic response)
        {
            string responseBody = JsonConvert.SerializeObject(response);
            return context.Response.WriteAsync(responseBody);
        }

        /// <summary>
        /// Add metadata to the http response
        /// </summary>
        /// <param name="context">the http context</param>
        /// <param name="metadata">the metadata</param>
        private static void AddMetadataToResponse(HttpContext context, dynamic metadata)
        {
            string stringifyMetadata = JsonConvert.SerializeObject(metadata);
            string metadataBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(stringifyMetadata));

            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("PACT_MESSAGE_METADATA", metadataBase64);
        }

        /// <summary>
        /// Read the request body as a string
        /// </summary>
        /// <param name="context">The http context</param>
        /// <returns>The request body</returns>
        private static async Task<string> GetInteractionDescriptionAsync(HttpContext context)
        {
            string requestBody;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            return JsonConvert.DeserializeObject<MessageInteraction>(requestBody)?.Description;
        }

        /// <summary>
        /// Write an error message in response
        /// </summary>
        /// <param name="context">The http context</param>
        /// <param name="errorMessage">The error message in the response content</param>
        /// <param name="statusCodeError">The status code related to the error</param>
        private static async Task WriteErrorInResponseAsync(HttpContext context, string errorMessage, HttpStatusCode statusCodeError)
        {
            context.Response.StatusCode = (int)statusCodeError;
            await WriteToResponseAsync(context, errorMessage);
        }
    }
}
