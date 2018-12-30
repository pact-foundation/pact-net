using System;
using ZooEventsConsumer.Models;

namespace ZooEventsConsumer
{
    public class SavePets
    {
        private IPetRepo _repo;

        public SavePets(IPetRepo repo)
        {
            _repo = repo;
        }

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
