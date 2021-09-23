using System;
using Microsoft.Extensions.Options;
using PactNet.Native.Options;

namespace PactNet.AspNetCore.Tests.Messaging
{
    internal class FakeMiddlewareOption : IOptionsMonitor<MessageMiddlewareOptions>
    {
        private readonly string basePathMessage;

        public FakeMiddlewareOption(string basePathMessage)
        {
            this.basePathMessage = basePathMessage;
        }

        public MessageMiddlewareOptions Get(string name)
        {
            throw new NotImplementedException();
        }

        public IDisposable OnChange(Action<MessageMiddlewareOptions, string> listener)
        {
            throw new NotImplementedException();
        }

        public MessageMiddlewareOptions CurrentValue => new() { BasePathMessage = basePathMessage };
    }
}
