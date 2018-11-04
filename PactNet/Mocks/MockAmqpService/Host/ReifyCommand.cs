using PactNet.Mocks.MockAmqpService.Models;

namespace PactNet.Mocks.MockAmqpService.Host
{
    internal class ReifyCommand : IPactMessageCommand
    {
        private readonly MessageInteraction _messageInteraction;

        public ReifyCommand(MessageInteraction messageInteraction, WaitForResultOutputter)
        {
            _messageInteraction = messageInteraction;
            _messageJson = messageJson;
        }

        public void Execute()
        {
            var arguments = 

            return $"reify '{_messageJson}'";
        }

        
    }
}
