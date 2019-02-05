using ZooEventsProducer.Models;

namespace ZooEventsProducer
{
    class Program
    {
        static void Main(string[] args)
        {
            //This is the message sender, who's sole purpose is to call the message generator and send the generated messages to the queue/stream/infrastructure.
            //You're messaging/event technology binding/wiring would go here.

            var animalCreator = new AnimalCreator();

            var message = animalCreator.CreateAPet("Fred", PetType.Fish);
            //Send to the queue. It would be nice to show a Rabbit/AMQP example?
        }
    }
}