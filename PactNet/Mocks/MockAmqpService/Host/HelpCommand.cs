namespace PactNet.Mocks.MockAmqpService.Host
{
    internal class HelpCommand : IPactMessageCommand
    {
        private readonly string _commandName;

        public HelpCommand(string commandName)
        {
            _commandName = commandName;
        }

        public string GetArguments()
        {
            return $"help {_commandName}";
        }
    }
}
