using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Consumer.Models;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace Consumer
{
    public class Reciever
    {
        public MessagedEvent GetMessage()
        {
            MessagedEvent consumedEvent = null;

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var json = Encoding.UTF8.GetString(body);
                    consumedEvent = JsonConvert.DeserializeObject<MessagedEvent>(json);
                };

                channel.BasicConsume(queue: "hello",
                                     noAck: true,
                                     consumer: consumer);
            }

            return consumedEvent;
        }
    }
}
