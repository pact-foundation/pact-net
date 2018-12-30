using PactNet.Infrastructure.Outputters;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ZooEventsConsumer.Tests
{
    public class XUnitOutput : IOutput
    {
        private readonly IMessageSink _sink;

        public XUnitOutput(IMessageSink sink)
        {
            _sink = sink;
        }

        public void WriteLine(string line)
        {
            _sink.OnMessage(new DiagnosticMessage(line));
        }
    }
}