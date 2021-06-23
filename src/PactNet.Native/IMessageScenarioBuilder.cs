using System;

namespace PactNet.Native
{
    public interface IMessageScenarioBuilder
    {
        IMessageScenarioBuilder WhenReceiving(string description);
        void WillPublishMessage(Func<dynamic> action);
        dynamic InvokeScenario(string description);
    }
}
