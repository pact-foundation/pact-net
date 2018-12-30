using NSubstitute;
using PactNet.PactMessage;
using Xunit;
using ZooEventsConsumer.Models;
using PactNet.PactMessage.Models;
using PactNet.Matchers;
using Xunit.Abstractions;

namespace ZooEventsConsumer.Tests
{
    public class SavePetsTests : IClassFixture<ConsumerEventPact>
    {
        private readonly IMessagePact _messagePact;

        public SavePetsTests(ConsumerEventPact data, ITestOutputHelper output)
        {
            _messagePact = data.Initialise(output);
        }

        [Fact]
        public void Handle_WhenAPetIsCreated_SavesThePet()
        {
            //Arrange
            var stubRepo = Substitute.For<IPetRepo>();
            var consumer = new SavePets(stubRepo);
            var pet = new Pet {Id = 1, Name = "Rover", Type = PetType.Dog};

            //TODO: Why is this an object?
            var providerStates = new[]
            {
                new ProviderState
                {
                    Name = "there is a Pet animal",
                }
            };

            //Act + Assert
            _messagePact.Given(providerStates)
                .ExpectedToReceive("a pet animal created event")
                .With(new Message
                {
                    Contents = new
                    {
                        id = Match.Type(pet.Id),
                        name = Match.Type(pet.Name),
                        type = Match.Regex(pet.Type.ToString(), "^(Dog|Cat|Fish)$")
                    }
                })
                .VerifyConsumer<AnimalCreated>(e => consumer.Handle(e)); //This also checks that no exceptions are thrown
            stubRepo.Received(1).SavePet(pet);
        }

        [Fact]
        public void Handle_WhenANonPetAnimalIsCreated_DoesNotSaveIt()
        {
            //Arrange
            var stubRepo = Substitute.For<IPetRepo>();
            var consumer = new SavePets(stubRepo);

            //TODO: Why is this an object?
            var providerStates = new[]
            {
                new ProviderState
                {
                    Name = "there is a non Pet animal",
                }
            };

            //Act + Assert
            _messagePact.Given(providerStates)
                .ExpectedToReceive("a non pet animal created event")
                .With(new Message
                {
                    Contents = new
                    {
                        id = Match.Type(2),
                        name = Match.Type("Terence"),
                        type = Match.Type("Giraffe")
                    }
                })
                .VerifyConsumer<AnimalCreated>(e => consumer.Handle(e)); //This also checks that no exceptions are thrown
            stubRepo.DidNotReceive().SavePet(Arg.Any<Pet>());
        }
    }
}
