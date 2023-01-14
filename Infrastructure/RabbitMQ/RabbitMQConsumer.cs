using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Infrastructure.RabbitMQ
{
    public class RabbitMQConsumer
    {
        readonly EventingBasicConsumer _consumer;
        protected readonly IModel _channel;

        public RabbitMQConsumer()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare("calculation", exclusive: false);
            _consumer = new EventingBasicConsumer(_channel);
        }

        public void AddListener()
        {
            _consumer.Received += (model, eventArgs) => {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Product message received: {message}");
            };
            _channel.BasicConsume(queue: "calculation", autoAck: true, consumer: _consumer);
        }
    }
}
