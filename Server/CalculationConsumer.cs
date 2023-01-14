using Application.DTOs.Requests;
using Application.Interfaces;
using Domain;
using Infrastructure.Services;
using MassTransit;

namespace Server
{
    public class CalculationConsumer: IConsumer<CalculationSettings>
    {
        private readonly ICalculationService _calculationService;

        public CalculationConsumer(ICalculationService calculationService)
        {
            _calculationService = calculationService;
        }

        public async Task Consume(ConsumeContext<CalculationSettings> context)
        {
            var calculationSettings = context.Message;
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch";
            await _calculationService.StartCalculation(context.Message, context.CancellationToken);
        }
    }
}
