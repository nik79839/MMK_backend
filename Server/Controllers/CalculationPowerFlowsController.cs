using DBRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Model;
using Model.Result;
using Model.Statistic;
using Server.Hub;

namespace Server.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class CalculationPowerFlowsController : ControllerBase
    {
        public RepositoryContext Db { get; set; }
        public IHubContext<ProgressHub> HubContext { get; set; }
        public CalculationPowerFlowsController(RepositoryContext context, IHubContext<ProgressHub> hubContext)
        {
            Db = context;
            this.HubContext = hubContext;
        }

        [Route("CalculationPowerFlows/PostCalculations")]
        [HttpPost]
        public void PostCalculations([FromBody] CalculationSettings calculationSettings)
        {
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            string guid = Guid.NewGuid().ToString();
            DateTime startTime = DateTime.UtcNow;
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch";;
            //List<int> nodesForWorsening = RastrManager.RayonNodesToList(rastr, 1); //Узлы района 1 (бодайб)
            
            Calculations calculations = new() { CalculationId = guid, Name = calculationSettings.Name, CalculationStart = startTime, CalculationEnd = null };
            calculations.CalculationProgress += EventHandler;
            Db.Calculations.Add(calculations);
            Db.SaveChanges();
            calculations.CalculatePowerFlows( calculationSettings, cancellationToken);
            DateTime endTime =DateTime.UtcNow;
            Console.WriteLine("Расчет завершен. Запись в БД.");

            Db.PowerFlowResults.AddRange(calculations.PowerFlowResults);
            Db.VoltageResults.AddRange(calculations.VoltageResults);
            calculations.CalculationEnd = endTime;
            Db.Calculations.Update(calculations);
            Db.SaveChanges();
            Console.WriteLine("Выполнена запись в БД");
        }

        [Route("CalculationPowerFlows/GetCalculations")]
        [HttpGet]
        public List<Calculations> GetCalculations()
        {
            return Db.Calculations.ToList().OrderByDescending(c => c.CalculationEnd).ToList();
        }

        [Route("CalculationPowerFlows/GetCalculations/{id}")]
        [HttpGet]
        public CalculationStatistic GetCalculationsById(string? id)
        {
            List<PowerFlowResult> powerFlowResults = (from calculations in Db.PowerFlowResults where calculations.CalculationId == id select calculations).ToList();
            List<VoltageResult> voltageResults = (from calculations in Db.VoltageResults where calculations.CalculationId == id select calculations).ToList();
            CalculationStatistic calculationStatistic = new();
            calculationStatistic.Processing(powerFlowResults);
            calculationStatistic.Processing(voltageResults);
            return calculationStatistic;
        }

        [Route("CalculationPowerFlows/DeleteCalculations/{id}")]
        [HttpDelete]
        public List<Calculations> DeleteCalculationsById(string? id)
        {
            Calculations calculations1 = (from calculations in Db.Calculations where calculations.CalculationId == id select calculations).FirstOrDefault();
            List<PowerFlowResult> calculationResults = (from calculations in Db.PowerFlowResults where calculations.CalculationId == id select calculations).ToList();
            List<VoltageResult> voltageResults = (from calculations in Db.VoltageResults where calculations.CalculationId == id select calculations).ToList();
            Db.Calculations.Remove(calculations1);
            Db.PowerFlowResults.RemoveRange(calculationResults);
            Db.VoltageResults.RemoveRange(voltageResults);
            Db.SaveChanges();
            return Db.Calculations.ToList();
        }

        public void EventHandler(object sender, CalculationProgressEventArgs e)
        {
            Console.WriteLine(e.Percent+"%, осталось "+e.Time+" мин");
            HubContext.Clients.All.SendAsync("SendProgress", e.Percent,e.CalculationId);
        }

    }
}