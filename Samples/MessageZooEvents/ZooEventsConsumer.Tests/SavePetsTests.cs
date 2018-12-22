using PactNet.PactMessage;
using Xunit;
using ZooEventsConsumer.Models;
using PactNet.PactMessage.Models;
using PactNet.Matchers;

namespace ZooEventsConsumer.Tests
{
    public class SavePetsTests : IClassFixture<ConsumerEventPact>
    {
        private readonly IMessagePact _messagePact;

        public SavePetsTests(ConsumerEventPact data)
        {
            _messagePact = data.MessagePact;
        }

        [Fact]
        public void Handle_WhenAPetIsCreated_SavesThePet()
        {
            //Arrange
            var stubRepo = new StubPetRepo();
            var consumer = new SavePets(stubRepo);

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
                .ExpectedToReceive("an animal created event")
                .With(new Message
                {
                    Contents = new
                    {
                        eventId = Match.Type(1),
                        name = Match.Type("Rover"),
                        type = Match.Type(PetType.Dog.ToString())
                    }
                })
                .VerifyConsumer<AnimalCreated>(e => consumer.Handle(e)); //This also checks that no exceptions are thrown
            Assert.True(stubRepo.SavePetWasCalled, "Pet was saved");
        }

        [Fact]
        public void Handle_WhenANonPetAnimalIsCreated_DoesNotSaveIt()
        {
            //Arrange
            var stubRepo = new StubPetRepo();
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
                        eventId = Match.Type(2),
                        name = Match.Type("Terence"),
                        type = Match.Type("Giraffe")
                    }
                })
                .VerifyConsumer<AnimalCreated>(e => consumer.Handle(e)); //This also checks that no exceptions are thrown
            Assert.False(stubRepo.SavePetWasCalled, "Animal was not saved");
        }
    }

    public class StubPetRepo : IPetRepo
    {
        public bool SavePetWasCalled { get; private set; }


        public void SavePet(Pet pet)
        {
            SavePetWasCalled = true;
        }
    }
}
