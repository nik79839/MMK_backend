using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Hub;

namespace Server.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
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

        [Route("CalculationPowerFlows/PostCalculations")]
        [HttpPost]
        public async Task<IActionResult> PostCalculations([FromBody] CalculationSettings calculationSettings)
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            string guid = Guid.NewGuid().ToString();
            DateTime startTime = DateTime.UtcNow;
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch"; ;
            //List<int> nodesForWorsening = RastrManager.RayonNodesToList(rastr, 1); //Узлы района 1 (бодайб)
            Calculations calculations = new() { Id = guid, Name = calculationSettings.Name, CalculationStart = startTime, CalculationEnd = null };
            //calculations.CalculationProgress += EventHandler;
            await _calculationService.StartCalculation(calculations, calculationSettings, cancellationToken);
            Console.WriteLine("Расчет завершен");
            return StatusCode(200, $"Расчет завершен.");
        }

        [Route("CalculationPowerFlows/GetCalculations")]
        [HttpGet]
        public async Task<IActionResult> GetCalculations()
        {
            return StatusCode(200, _calculationService.GetCalculations());
        }

        [Route("CalculationPowerFlows/GetCalculations/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCalculationsById(string? id)
        {
            return StatusCode(200, _calculationService.GetCalculationsById(id));
        }

        [Route("CalculationPowerFlows/DeleteCalculations/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCalculationsById(string? id)
        {
            await _calculationService.DeleteCalculationById(id);
            return StatusCode(200);
        }

        public void EventHandler(object sender, CalculationProgressEventArgs e)
        {
            Console.WriteLine(e.Percent+"%, осталось "+e.Time+" мин");
            _hubContext.Clients.All.SendAsync("SendProgress", e.Percent,e.CalculationId);
        }

    }
}