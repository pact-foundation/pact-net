using PactNet.Mocks.MockAmqpService;
using Xunit;
using Xunit.Sdk;

namespace PactNet.Tests.Mocks.MockAmqpService
{
    public class RabbitMqMockProviderServiceTests
    {
        private RabbitMqMockProviderService _rabbitMqMockProviderService;

        public RabbitMqMockProviderServiceTests()
        {
            _rabbitMqMockProviderService = new RabbitMqMockProviderService();
        }

        [Fact]
        public void Given_WithProviderState_SetsProviderState()
        {
            //Arrange
            const string providerState = "My provider state";

            //Act
            _rabbitMqMockProviderService.Given(providerState)
                .Given(providerState);

            //Assert
            _rabbitMqMockProviderService.VerifyPublishing();
//            _mockedHost.Recieved(1).PublishMessage(null);
        }
    }
}
