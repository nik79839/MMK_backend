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
            DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_���.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_�������.sch";;
            //List<int> nodesForWorsening = RastrManager.RayonNodesToList(rastr, 1); //���� ������ 1 (������)
            
            Calculations calculations = new() { CalculationId = guid, Name = calculationSettings.Name, CalculationStart = startTime, CalculationEnd = null };
            calculations.CalculationProgress += EventHandler;
            Db.Calculations.Add(calculations);
            Db.SaveChanges();
            calculations.CalculatePowerFlows( calculationSettings, cancellationToken);
            DateTime endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second); ;
            Console.WriteLine("������ ��������. ������ � ��.");

            Db.CalculationResults.AddRange(calculations.CalculationResults);
            Db.VoltageResults.AddRange(calculations.VoltageResults);
            calculations.CalculationEnd = endTime;
            Db.Calculations.Update(calculations);
            Db.SaveChanges();
            Console.WriteLine("��������� ������ � ��");
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
            List<CalculationResult> calculationResults = (from calculations in Db.CalculationResults where calculations.CalculationId == id select calculations).ToList();
            List<VoltageResult> voltageResults = (from calculations in Db.VoltageResults where calculations.CalculationId == id select calculations).ToList();
            CalculationStatistic calculationStatistic = new();
            calculationStatistic.Processing(calculationResults);
            calculationStatistic.Processing(voltageResults);
            return calculationStatistic;
        }

        [Route("CalculationPowerFlows/DeleteCalculations/{id}")]
        [HttpDelete]
        public List<Calculations> DeleteCalculationsById(string? id)
        {
            Calculations calculations1 = (from calculations in Db.Calculations where calculations.CalculationId == id select calculations).FirstOrDefault();
            List<CalculationResult> calculationResults = (from calculations in Db.CalculationResults where calculations.CalculationId == id select calculations).ToList();
            List<VoltageResult> voltageResults = (from calculations in Db.VoltageResults where calculations.CalculationId == id select calculations).ToList();
            Db.Calculations.Remove(calculations1);
            Db.CalculationResults.RemoveRange(calculationResults);
            Db.VoltageResults.RemoveRange(voltageResults);
            Db.SaveChanges();
            return Db.Calculations.ToList();
        }

        public void EventHandler(object sender, CalculationProgressEventArgs e)
        {
            Console.WriteLine(e.Percent+"%, �������� "+e.Time+" ���");
            HubContext.Clients.All.SendAsync("SendProgress", e.Percent,e.CalculationId);
        }

    }
}