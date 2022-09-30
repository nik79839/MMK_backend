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
            CancellationToken cancellationToken = HttpContext.RequestAborted;
            string guid = Guid.NewGuid().ToString();
            DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_���.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_�������.sch";;
            //List<int> nodesForWorsening = RastrManager.RayonNodesToList(rastr, 1); //���� ������ 1 (������)
            
            Calculations calculations = new() { CalculationId = guid, Name = calculationSettings.Name, CalculationStart = startTime, CalculationEnd = null };
            calculations.CalculationProgress += EventHandler;
            db.Calculations.Add(calculations);
            db.SaveChanges();
            calculations.CalculatePowerFlows( calculationSettings, cancellationToken);
            DateTime endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second); ;
            Console.WriteLine("������ ��������. ������ � ��.");

            db.CalculationResults.AddRange(calculations.CalculationResults);
            db.VoltageResults.AddRange(calculations.VoltageResults);
            calculations.CalculationEnd = endTime;
            db.Calculations.Update(calculations);
            db.SaveChanges();
            Console.WriteLine("��������� ������ � ��");
        }

        [Route("CalculationPowerFlows/GetCalculations")]
        [HttpGet]
        public List<Calculations> GetCalculations()
        {
            return db.Calculations.ToList().OrderByDescending(c => c.CalculationEnd).ToList();
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

        [Route("CalculationPowerFlows/DeleteCalculations/{id}")]
        [HttpDelete]
        public List<Calculations> DeleteCalculationsById(string? id)
        {
            Calculations calculations1 = (from calculations in db.Calculations where calculations.CalculationId == id select calculations).FirstOrDefault();
            List<CalculationResult> calculationResults = (from calculations in db.CalculationResults where calculations.CalculationId == id select calculations).ToList();
            List<VoltageResult> voltageResults = (from calculations in db.VoltageResults where calculations.CalculationId == id select calculations).ToList();
            db.Calculations.Remove(calculations1);
            db.CalculationResults.RemoveRange(calculationResults);
            db.VoltageResults.RemoveRange(voltageResults);
            db.SaveChanges();
            return db.Calculations.ToList();
        }

        public void EventHandler(object sender, CalculationProgressEventArgs e)
        {
            Console.WriteLine(e.Percent+"%, �������� "+e.Time+" ���");
            hubContext.Clients.All.SendAsync("SendProgress", e.Percent,e.CalculationId);
        }

    }
}