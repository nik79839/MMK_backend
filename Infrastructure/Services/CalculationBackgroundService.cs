using Application.Interfaces;
using Infrastructure.RabbitMQ;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services
{
    public class CalculationBackgroundService: BackgroundService
    {
        readonly RabbitMQConsumer consumer;
        public CalculationBackgroundService(RabbitMQConsumer consumer)
        {
            this.consumer = consumer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            consumer.AddListener();
            return Task.CompletedTask;
        }

    }
}
