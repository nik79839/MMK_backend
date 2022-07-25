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
            Rastr rastr = new Rastr();
            string guid = Guid.NewGuid().ToString();
            DateTime startTime = DateTime.Now;
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_���.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_�������.sch";
            rastr.Load(RG_KOD.RG_REPL, calculationSettings.PathToRegim, calculationSettings.PathToRegim);
            Dictionary<string, List<double>> UValueDict = new Dictionary<string, List<double>>();
            calculationSettings.LoadNodes = new List<int>() //������ ����� ��������
            {
                1620, 1610, 2643,
                2605, 1800,1654,1619,60408134,60408133,2630,60405013,1618,1616,2652,60405014,2644,60408115,60408116,2641,60408123,
                60408124,60408125,60408126,60408122,60408121,60408120,60408119,2631,1617,641,2642,640,60405020,2653,1655,2647,2648,2649,
                2639,60405028,1805,2645,2646,60405012,599,600,60405029,2603
            };
            //List<int> nodesForWorsening = RastrManager.RayonNodesToList(rastr, 1); //���� ������ 1 (������)
            calculationSettings.NodesForWorsening = RastrManager.RayonNodesToList(rastr, 1).Union(new List<int>() { 1654 }).ToList();
         
            Calculations calculations = new() { CalculationId = guid, Name = calculationSettings.Name, CalculationStart = startTime, CalculationEnd = null };
            calculations.CalculationProgress += EventHandler;
            db.Calculations.Add(calculations);
            db.SaveChanges();
            calculations.CalculatePowerFlows(rastr, calculationSettings);
            DateTime endTime = DateTime.Now;
            Console.WriteLine("������ ��������. ������ � ��.");
            //FileManager.ToExcel(powerFlows, 6, UValueDict);
            //FileManager.ToExcel_I(IValueDict);
            db.CalculationResults.AddRange(calculations.CalculationResults);
            calculations.CalculationEnd = endTime;
            db.Calculations.Update(calculations);
            db.SaveChanges();
            Console.WriteLine("��������� ������ � ��");
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
            CalculationStatistic calculationStatistic = new();
            calculationStatistic.Processing(calculationResults);
            return calculationStatistic;
        }

        public void EventHandler(object sender, CalculationProgressEventArgs e)
        {
            Console.WriteLine(e.Percent+"%, �������� "+e.Time+" ���");
            hubContext.Clients.All.SendAsync("SendProgress", e.Percent,e.CalculationId);
        }

    }
}