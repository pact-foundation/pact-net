namespace PactNet.Mocks.MockAmqpService.Host
{
    internal class VersionCommand : IPactMessageCommand
    {
        public string GetArguments()
        {
            return "version";
        }
    }
}
