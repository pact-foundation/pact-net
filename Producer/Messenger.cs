using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Producer.Models;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace Producer
{
    public class Messenger
    {
        public void BroadcastEvent(string topic, Event eventToBroadcast)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    string json = JsonConvert.SerializeObject(eventToBroadcast);

                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "",
                                         routingKey: topic,
                                         basicProperties: null,
                                         body: body);
                }
            }
        }

    }
}
