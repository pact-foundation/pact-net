using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PactNet.AspNetCore.Messaging;
using PactNet.AspNetCore.Tests.Mock;

namespace PactNet.AspNetCore.Tests.Messaging
{
    internal class FakeMessageMiddleware : MessageMiddleware
    {
        public FakeHttpContext Context { get; }
        private readonly dynamic scenarioReturned;

        public object ResponseWritten { get; set; }

        public FakeMessageMiddleware(FakeMiddlewareOption options, dynamic scenarioReturned, RequestDelegate next)
            : base(options, next)
        {
            this.scenarioReturned = scenarioReturned;
        }

        public FakeMessageMiddleware(FakeHttpContext httpContext, FakeMiddlewareOption options, dynamic scenarioReturned, RequestDelegate next)
            : base(options, next)
        {
            this.Context = httpContext;
            this.scenarioReturned = scenarioReturned;
        }

        protected override Task WriteToResponseAsync(HttpContext context, dynamic response)
        {
            ResponseWritten = response;
            return Task.CompletedTask;
        }

        protected override dynamic InvokeScenario(string interactionDescription)
        {
            return scenarioReturned;
        }

        public async Task DoInvokeAsync()
        {
            await InvokeAsync(Context);
        }
    }
}
