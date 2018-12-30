using ZooEventsConsumer.Models;

namespace ZooEventsConsumer
{
    public interface IPetRepo
    {
        void SavePet(Pet pet);
    }
}
