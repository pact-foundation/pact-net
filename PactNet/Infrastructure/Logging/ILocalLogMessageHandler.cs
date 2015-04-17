using System;

namespace PactNet.Infrastructure.Logging
{
    internal interface ILocalLogMessageHandler : IDisposable
    {
        void Handle(LocalLogMessage logMessage);
    }
}