using ZooEventsProducer.Models;

namespace ZooEventsProducer
{
    public class AnimalCreator
    {
        private int _currentId = 1;

        //This is the message generator, which contains any message generation domain logic.

        public AnimalCreated CreateAPet(string name, PetType type)
        {
            return new AnimalCreated
            {
                Id = _currentId++,
                Name = name,
                Type = type.ToString()
            };
        }

        //Another message generator

        public AnimalCreated CreateANonPet(string name, string type)
        {
            return new AnimalCreated
            {
                Id = _currentId++,
                Name = name,
                Type = type
            };
        }
    }
}
