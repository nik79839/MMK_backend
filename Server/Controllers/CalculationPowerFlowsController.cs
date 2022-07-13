using ASTRALib;
using DBRepository;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace Server.Controllers
{
    //[ApiController]
    //[Route("[controller]")]
    public class CalculationPowerFlowsController : ControllerBase
    {
        RepositoryContext db;
        public CalculationPowerFlowsController(RepositoryContext context)
        {
            db = context;
        }

        [Route("CalculationPowerFlows/PostCalculations")]
        [HttpPost]
        public void PostCalculations(CalculationSettings calculationSettings)
        {
            Rastr rastr = new Rastr();
            DateTime startTime = DateTime.Now;
            Console.WriteLine(calculationSettings.CountOfImplementations);
            calculationSettings.PathToRegim = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_���.rg2";
            calculationSettings.PathToSech = @"C:\Users\otrok\Desktop\����� ����\������_��\���������\���\����_�������.sch";
            rastr.Load(RG_KOD.RG_REPL, calculationSettings.PathToRegim, calculationSettings.PathToRegim);
            Dictionary<string, List<double>> UValueDict = new Dictionary<string, List<double>>();
            calculationSettings.NagrNodes = new List<int>() //������ ����� ��������
            {
                1620, 1610, 2643,
                2605, 1800,1654,1619,60408134,60408133,2630,60405013,1618,1616,2652,60405014,2644,60408115,60408116,2641,60408123,
                60408124,60408125,60408126,60408122,60408121,60408120,60408119,2631,1617,641,2642,640,60405020,2653,1655,2647,2648,2649,
                2639,60405028,1805,2645,2646,60405012,599,600,60405029,2603
            };
            //List<int> nodesForWorsening = RastrManager.RayonNodesToList(rastr, 1); //���� ������ 1 (������)
            calculationSettings.NodesForWorsening = RastrManager.RayonNodesToList(rastr, 1).Union(new List<int>() { 1654 }).ToList();
            List<CalculationResult> powerFlows = Calculation.CalculatePowerFlows(rastr, calculationSettings);
            DateTime endTime = DateTime.Now;
            Console.WriteLine("������ ��������. ������ � ��.");
            //FileManager.ToExcel(powerFlows, 6, UValueDict);
            //FileManager.ToExcel_I(IValueDict);
            Calculations calculations = new() { CalculationId = powerFlows[0].CalculationId, Name = calculationSettings.Name, CalculationStart = startTime, CalculationEnd = endTime };
            db.Calculations.Add(calculations);
            db.CalculationResults.AddRange(powerFlows);            
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
        public List<Calculations> GetCalculationsById(int? id)
        {
            Console.WriteLine(id);
            return db.Calculations.ToList();
        }

    }
}