using System;
using PactNet.Mocks.MockAmqpService.Host;
using PactNet.Mocks.MockAmqpService.Models;

namespace PactNet.Mocks.MockAmqpService
{
    public class PactMessage : IPactMessage
    {
        private string _providerState;
        private string _description;
        private MessageInteraction _messageInteraction;
        private string _consumerName;
        private PactConfig _providerName;

        public IPactMessage ExpectedToReceive(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("Please supply a non null or empty description");
            }

            _description = description;

            return this;
        }

        public IPactMessage Given(string providerState)
        {
            if (string.IsNullOrEmpty(providerState))
            {
                throw new ArgumentException("Please supply a non null or empty providerState");
            }

            _providerState = providerState;

            return this;
        }

        public IPactMessage With(MessageInteraction messageInteraction)
        {                
            _messageInteraction = messageInteraction ?? throw new ArgumentException("Please supply a non null message");

            return this;
        }

        public void VerifyConsumer(Action<string> messageHandler)
        {
            var waitForResultOutputer = new WaitForResultOutputter();

            var reifyAction = new ReifyCommand(_messageInteraction, waitForResultOutputer);
            reifyAction.Execute();

            var message = waitForResultOutputer.FullOutput;
            messageHandler(message);

            new UpdateCommand(_messageInteraction, _consumerName, _providerName, _pactConfig).Execute();
        }
    }
}
