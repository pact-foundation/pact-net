using PactNet.Mocks.MockAmqpService;
using Xunit;
using Xunit.Sdk;

namespace PactNet.Tests.Mocks.MockAmqpService
{
    public class RabbitMqMockProviderServiceTests
    {
        private PactMessage _pactMessage;

        public RabbitMqMockProviderServiceTests()
        {
            _pactMessage = new PactMessage();
        }

        [Fact]
        public void Given_WithProviderState_SetsProviderState()
        {
            //Arrange
            const string providerState = "My provider state";

            //Act
            _pactMessage.Given(providerState)
                .Given(providerState);

            //Assert
            _pactMessage.VerifyConsumer();
//            _mockedHost.Recieved(1).PublishMessage(null);
        }
    }
}
