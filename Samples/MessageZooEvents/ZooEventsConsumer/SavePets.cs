using System;
using ZooEventsConsumer.Models;

namespace ZooEventsConsumer
{
    public class SavePets
    {
        private readonly IPetRepo _repo;

        public SavePets(IPetRepo repo)
        {
            _repo = repo;
        }

        //This is the message handler, which processes the message and performs the actual domain logic.

        public void Handle(AnimalCreated @event)
        {
            var isPet = Enum.TryParse(@event.Type, true, out PetType petType);
            if (isPet)
            {
                _repo.SavePet(new Pet
                {
                    Id = @event.Id,
                    Name = @event.Name,
                    Type = petType
                });
            }
        }            
    }
}
