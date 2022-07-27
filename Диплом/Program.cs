using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ASTRALib;
using Model;

namespace Диплом_УР_Автоматизация
{
    class Program
    {
        static void Main(string[] args)
        {
            Rastr rastr = new Rastr();
            CalculationSettings calculationSettings = new CalculationSettings();
            calculationSettings.CountOfImplementations = 100;
            calculationSettings.PathToRegim= @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_СХН.rg2";
            calculationSettings.PathToSech= @"C:\Users\otrok\Desktop\Файлы ворд\Диплом_УР\Дипломмаг\Мой\СБЭК_сечения.sch";
            rastr.Load(RG_KOD.RG_REPL, calculationSettings.PathToRegim, calculationSettings.PathToRegim);
            Dictionary<string, List<double>> UValueDict = new Dictionary<string, List<double>>();
            calculationSettings.LoadNodes = RastrManager.AllLoadNodesToList(rastr); //Список узлов нагрузки
            //List<int> nodesForWorsening = RastrManager.RayonNodesToList(rastr, 1); //Узлы района 1 (бодайб)
            calculationSettings.NodesForWorsening = RastrManager.DistrictNodesToList(rastr, 1).Union(new List<int>() { 1654 }).ToList();
            calculationSettings.PercentForWorsening = 10;
            calculationSettings.PercentLoad = 50;
            Calculations calculations = new() { CalculationId = "sdgsdh", Name = calculationSettings.Name, CalculationEnd = null };
            calculations.CalculationProgress += EventHandler;
            calculationSettings.NodesForWorsening = RastrManager.DistrictNodesToList(rastr, 1).Union(new List<int>() { 1654 }).ToList();
            calculationSettings.SechNumber = 1;
            //db.SaveChanges();
            calculations.CalculatePowerFlows(rastr, calculationSettings);
            //Calculations.CalculationProgress += EventHandler;
            //List<CalculationResult> powerFlows = Calculations.CalculatePowerFlows(rastr, calculationSettings);
            //List<double> powerFlows=Calculation.CalculatePowerFlows(rastr, calculationSettings);


            Console.WriteLine("Расчет завершен, нажмите любую кнопку");
            Console.ReadKey();
            //FileManager.ToExcel(powerFlows, 6, UValueDict);
            //FileManager.ToExcel_I(IValueDict);
            
        }
        public static void EventHandler(object sender, CalculationProgressEventArgs e)
        {
            Console.WriteLine(e.Percent);
        }
    }
}
