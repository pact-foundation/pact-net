namespace PactNet.Mocks.MockAmqpService.Host
{
    internal class UpdateCommand : IPactMessageCommand
    {
        private readonly string _consumer;
        private readonly string _provider;
        private readonly PactConfig _pactConfig;
        private readonly string _messageJson;

        public UpdateCommand(string consumer, string provider, PactConfig pactConfig, string messageJson)
        {
            _consumer = consumer;
            _provider = provider;
            _pactConfig = pactConfig;
            _messageJson = messageJson;
        }

        public void Execute()
        {
            return $"update '{_messageJson}' --consumer={_consumer} --provider={_provider} " +
                   $"--pactdir={_pactConfig.PactDir} --specificationversion={_pactConfig.SpecificationVersion}";
        }
    }
}
