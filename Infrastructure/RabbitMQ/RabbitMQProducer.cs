using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Infrastructure.RabbitMQ
{
    public class RabbitMQProducer: IRabbitMQProducer
    {
        public void SendProductMessage<T>(T message)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            var connection = factory.CreateConnection();
            using
            var channel = connection.CreateModel();
            channel.QueueDeclare("calculation", exclusive: false);
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: "", routingKey: "calculation", body: body);
        }
    }
}
