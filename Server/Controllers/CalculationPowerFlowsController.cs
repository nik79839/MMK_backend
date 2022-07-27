using ASTRALib;
using DBRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Model;
using Server.Hub;

namespace Server.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class CalculationPowerFlowsController : ControllerBase
    {
        RepositoryContext db;
        IHubContext<ProgressHub> hubContext;
        public CalculationPowerFlowsController(RepositoryContext context, IHubContext<ProgressHub> hubContext)
        {
            db = context;
            this.hubContext = hubContext;
        }

        [Route("CalculationPowerFlows/PostCalculations")]
        [HttpPost]
        public void PostCalculations(CalculationSettings calculationSettings)
        {
            Rastr rastr = new();
            string guid = Guid.NewGuid().ToString();
            DateTime startTime = DateTime.Now;
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch";
            rastr.Load(RG_KOD.RG_REPL, calculationSettings.PathToRegim, calculationSettings.PathToRegim);
            if (calculationSettings.IsAllNodesInitial)
            {
                calculationSettings.LoadNodes = RastrManager.AllLoadNodesToList(rastr); //Список узлов нагрузки со случайными начальными параметрами (все узлы)
            }
            //List<int> nodesForWorsening = RastrManager.RayonNodesToList(rastr, 1); //Узлы района 1 (бодайб)
            calculationSettings.NodesForWorsening = RastrManager.DistrictNodesToList(rastr, 1).Union(new List<int>() { 1654 }).ToList();
         
            Calculations calculations = new() { CalculationId = guid, Name = calculationSettings.Name, CalculationStart = startTime, CalculationEnd = null };
            calculations.CalculationProgress += EventHandler;
            db.Calculations.Add(calculations);
            db.SaveChanges();
            calculations.CalculatePowerFlows(rastr, calculationSettings);
            DateTime endTime = DateTime.Now;
            Console.WriteLine("Расчет завершен. Запись в БД.");
            //FileManager.ToExcel(powerFlows, 6, UValueDict);
            //FileManager.ToExcel_I(IValueDict);
            db.CalculationResults.AddRange(calculations.CalculationResults);
            db.VoltageResults.AddRange(calculations.VoltageResults);
            calculations.CalculationEnd = endTime;
            db.Calculations.Update(calculations);
            db.SaveChanges();
            Console.WriteLine("Выполнена запись в БД");
        }

        [Route("CalculationPowerFlows/GetCalculations")]
        [HttpGet]
        public List<Calculations> GetCalculations()
        {
            return db.Calculations.ToList();
        }

        [Route("CalculationPowerFlows/GetCalculations/{id}")]
        [HttpGet]
        public CalculationStatistic GetCalculationsById(string? id)
        {
            List<CalculationResult> calculationResults = (from calculations in db.CalculationResults where calculations.CalculationId == id select calculations).ToList();
            List<VoltageResult> voltageResults = (from calculations in db.VoltageResults where calculations.CalculationId == id select calculations).ToList();
            CalculationStatistic calculationStatistic = new();
            calculationStatistic.Processing(calculationResults);
            calculationStatistic.Processing(voltageResults);
            return calculationStatistic;
        }

        public void EventHandler(object sender, CalculationProgressEventArgs e)
        {
            Console.WriteLine(e.Percent+"%, осталось "+e.Time+" мин");
            hubContext.Clients.All.SendAsync("SendProgress", e.Percent,e.CalculationId);
        }

    }
}