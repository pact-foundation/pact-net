using System;
using ZooEventsConsumer.Models;

namespace ZooEventsConsumer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var handler = new SavePets(new PetRepo());

            //This is the message listener, who's sole purpose is to receive the message from the queue/stream/infrastructure and forward onto the correct message handler.
            //You're messaging/event technology binding/wiring would go here.

            //Received from the Queue. It would be nice to show a Rabbit/AMQP example?
            var message = new AnimalCreated
            {
                Id = 1234,
                Name = "Fred",
                Type = PetType.Fish.ToString()
            };

            handler.Handle(message);
        }
    }
}