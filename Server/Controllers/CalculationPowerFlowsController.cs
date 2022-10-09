using Data;
using Data.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using BLL;
using BLL.Events;
using BLL.Result;
using BLL.Statistic;
using Server.Hub;

namespace Server.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class CalculationPowerFlowsController : ControllerBase
    {
        private IHubContext<ProgressHub> _hubContext;
        private ICalculationResultRepository _repository;
        public CalculationPowerFlowsController(IHubContext<ProgressHub> hubContext, ICalculationResultRepository repository)
        {
            _hubContext = hubContext;
            _repository = repository;
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

            Calculations calculations = new() { CalculationId = guid, Name = calculationSettings.Name, CalculationStart = startTime, CalculationEnd = null };
            calculations.CalculationProgress += EventHandler;
            await _repository.AddCalculation(calculations);
            calculations.CalculatePowerFlows(calculationSettings, cancellationToken);
            DateTime endTime = DateTime.UtcNow;
            Console.WriteLine("Расчет завершен. Запись в БД.");

            await _repository.AddPowerFlowResults(calculations.PowerFlowResults);
            await _repository.AddVoltageResults(calculations.VoltageResults);
            calculations.CalculationEnd = endTime;
            await _repository.UpdateCalculation(calculations);
            Console.WriteLine("Выполнена запись в БД");
            return StatusCode(200, $"Расчет завершен.");
        }

        [Route("CalculationPowerFlows/GetCalculations")]
        [HttpGet]
        public async Task<IActionResult> GetCalculations()
        {
            return StatusCode(200, _repository.GetCalculations().Result);
        }

        [Route("CalculationPowerFlows/GetCalculations/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCalculationsById(string? id)
        {
            List<PowerFlowResult> powerFlowResults = _repository.GetPowerFlowResultById(id).Result;
            List<VoltageResult> voltageResults = _repository.GetVoltageResultById(id).Result;
            CalculationStatistic calculationStatistic = new();
            if (powerFlowResults.Count == 0)
            {
                return StatusCode(400, $"Ошибка. Расчета с ID {id} не существует.");
            }
            calculationStatistic.Processing(powerFlowResults);
            calculationStatistic.Processing(voltageResults);
            return StatusCode(200, calculationStatistic);
        }

        [Route("CalculationPowerFlows/DeleteCalculations/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCalculationsById(string? id)
        {
            await _repository.DeleteCalculationsById(id);
            return StatusCode(200, _repository.GetCalculations().Result);
        }

        public void EventHandler(object sender, CalculationProgressEventArgs e)
        {
            Console.WriteLine(e.Percent+"%, осталось "+e.Time+" мин");
            _hubContext.Clients.All.SendAsync("SendProgress", e.Percent,e.CalculationId);
        }

    }
}