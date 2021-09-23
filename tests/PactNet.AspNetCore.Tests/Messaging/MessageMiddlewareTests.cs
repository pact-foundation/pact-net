using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using PactNet.AspNetCore.Tests.Mock;
using Xunit;

namespace PactNet.AspNetCore.Tests.Messaging
{

    /// <summary>
    /// Defines the messaging middleware behaviors
    /// </summary>
    public class MessageMiddlewareTests : BaseMiddlewareTest
    {
        /// <summary>
        /// Message middleware should handle only the pact message route
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_handle_only_pact_message_route()
        {
            var fakeHttpContext = new FakeHttpContext(HttpMethod.Get, "/another-route", null, HttpStatusCode.OK);

            var middleware = CreateMiddleware("/message-route");

            await middleware.InvokeAsync(fakeHttpContext);

            NextTriggered.Should().Be(true);
        }

        /// <summary>
        /// Message middleware should indicate if an invalid request has been sent
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_indicate_if_an_invalid_request_has_been_sent()
        {
            string messageRoute = "/message-route";
            var fakeHttpContext = new FakeHttpContext(
                HttpMethod.Get,
                messageRoute,
                new { wrongField = "a value" },
                HttpStatusCode.OK);

            var middleware = CreateMiddleware(messageRoute);

            await middleware.InvokeAsync(fakeHttpContext);

            NextTriggered.Should().Be(false);
            fakeHttpContext.Response.StatusCode.Should().Be(400);
            fakeHttpContext.Response.Body.Should().BeNull();
        }

        /// <summary>
        /// Message middleware should indicate if scenario is not found
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_indicate_if_scenario_is_not_found()
        {
            string messageRoute = "/message-route";
            var fakeHttpContext = new FakeHttpContext(
                HttpMethod.Get,
                messageRoute,
                new { Description = "a scenario" },
                HttpStatusCode.OK);

            var middleware = CreateMiddleware(messageRoute);

            await middleware.InvokeAsync(fakeHttpContext);

            NextTriggered.Should().Be(false);
            fakeHttpContext.Response.StatusCode.Should().Be(404);
            fakeHttpContext.Response.Body.Should().BeNull();
        }

        /// <summary>
        /// Message middleware should invoke scenario and return result
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Should_invoke_scenario_and_return_messaging_result()
        {
            string messageRoute = "/message-route";

            dynamic myObjectReturned = new { field = "valueField" };
            FakeMessageMiddleware middleware =
                CreateMiddleware(messageRoute, new { Description = "a scenario" }, myObjectReturned);

            await middleware.DoInvokeAsync();

            NextTriggered.Should().Be(false);
            middleware.Context.Response.StatusCode.Should().Be(200);
            middleware.ResponseWritten.Should().Be(myObjectReturned);
        }

        /// <summary>
        /// Create the fake middleware
        /// </summary>
        /// <param name="messageRoute">which route to process</param>
        /// <returns>The middleware object</returns>
        private FakeMessageMiddleware CreateMiddleware(string messageRoute)
        {
            return new FakeMessageMiddleware(new FakeMiddlewareOption(messageRoute), null, NextHandle);
        }

        /// <summary>
        /// Create the fake middleware
        /// </summary>
        /// <param name="messageRoute">which route to process</param>
        /// <param name="objectReturned">the object returned after scenario is invoked</param>
        /// <returns>The middleware object</returns>
        private FakeMessageMiddleware CreateMiddleware(string messageRoute, dynamic objectReturned)
        {
            return new FakeMessageMiddleware(new FakeMiddlewareOption(messageRoute), objectReturned, NextHandle);
        }

        /// <summary>
        /// Create the fake middleware
        /// </summary>
        /// <param name="messageRoute">which route to process</param>
        /// <param name="requestBody">The request body</param>
        /// <param name="objectReturned">the object returned after scenario is invoked</param>
        /// <returns>The middleware object</returns>
        private FakeMessageMiddleware CreateMiddleware(string messageRoute, dynamic requestBody, dynamic objectReturned)
        {
            var fakeHttpContext = new FakeHttpContext(
                HttpMethod.Get,
                messageRoute,
                requestBody,
                HttpStatusCode.OK);

            return new FakeMessageMiddleware(fakeHttpContext, new FakeMiddlewareOption(messageRoute), objectReturned, NextHandle);
        }
    }
}
