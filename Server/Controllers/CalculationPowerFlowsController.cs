using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Hub;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculationPowerFlowsController : ControllerBase
    {
        private readonly IHubContext<ProgressHub> _hubContext;
        private readonly ICalculationService _calculationService;
        private readonly IMapper _mapper;
        public CalculationPowerFlowsController(IHubContext<ProgressHub> hubContext, ICalculationService calculationService, IMapper mapper)
        {
            _hubContext = hubContext;
            _mapper = mapper;
            _calculationService = calculationService;
        }

        [HttpPost]
        [Route("PostCalculations")]
        public async Task<IActionResult> PostCalculations([FromBody]CalculationSettings calculationSettings)
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            string guid = Guid.NewGuid().ToString();
            DateTime startTime = DateTime.UtcNow;
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch"; ;
            Calculations calculations = new() { Id = guid, Name = calculationSettings.Name, Description = calculationSettings.Description,
                CalculationStart = startTime, CalculationEnd = null, PathToRegim = calculationSettings.PathToRegim, PercentLoad = calculationSettings.PercentLoad,
                PercentForWorsening = calculationSettings.PercentForWorsening
            };
            _calculationService.CalculationProgress += EventHandler;
            await _calculationService.StartCalculation(calculations, calculationSettings, cancellationToken);
            Console.WriteLine("Расчет завершен");
            return Ok($"Расчет завершен.");
        }

        [HttpGet]
        [Route("GetCalculations")]
        public async Task<IActionResult> GetCalculations()
        {
            return Ok(_calculationService.GetCalculations());
        }

        [HttpGet]
        [Route("GetCalculations/{id}")]
        public async Task<IActionResult> GetCalculationsById(string? id)
        {
            return Ok(_calculationService.GetCalculationsById(id));
        }

        [HttpDelete]
        [Route("DeleteCalculations/{id}")]
        public async Task<IActionResult> DeleteCalculationsById(string? id)
        {
            await _calculationService.DeleteCalculationById(id);
            return Ok;
        }

        [NonAction]
        public void EventHandler(object sender, CalculationProgressEventArgs e)
        {
            Console.WriteLine(e.Percent + "%, осталось " + e.Time + " мин");
            _hubContext.Clients.All.SendAsync("SendProgress", e.Percent, e.CalculationId);
        }

    }
}