using ZooEventsProducer.Models;

namespace ZooEventsProducer
{
    public class AnimalCreator
    {
        private int _currentId = 1;

        public AnimalCreated CreateAPet(string name, PetType type)
        {
            return new AnimalCreated
            {
                Id = _currentId++,
                Name = name,
                Type = type.ToString()
            };
        }

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
