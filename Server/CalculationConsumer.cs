using Application.DTOs.Requests;
using Application.Interfaces;
using Domain;
using Domain.Events;
using Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Server.Hub;

namespace Server
{
    public class CalculationConsumer: IConsumer<CalculationSettings>
    {
        private readonly ICalculationService _calculationService;
        private readonly IHubContext<ProgressHub> _hubContext;

        public CalculationConsumer(ICalculationService calculationService, IHubContext<ProgressHub> hubContext)
        {
            _calculationService = calculationService;
            _hubContext = hubContext;
            _calculationService.CalculationProgress += EventHandler;
        }

        public async Task Consume(ConsumeContext<CalculationSettings> context)
        {
            Console.WriteLine("Начало расчета");
            var calculationSettings = context.Message;
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch";
            await _calculationService.StartCalculation(context.Message, context.CancellationToken);
            Console.WriteLine("Конец расчета");
        }

        public void EventHandler(object sender, CalculationProgressEventArgs e)
        {
            Console.WriteLine(e.Percent + "%, осталось " + e.Time + " мин");
            _hubContext.Clients.All.SendAsync("SendProgress", e.Percent, e.CalculationId);
        }
    }
}
