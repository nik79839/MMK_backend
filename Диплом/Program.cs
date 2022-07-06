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
            calculationSettings.NagrNodes = new List<int>() //Список узлов нагрузки
            {
                1620, 1610, 2643,
                2605, 1800,1654,1619,60408134,60408133,2630,60405013,1618,1616,2652,60405014,2644,60408115,60408116,2641,60408123,
                60408124,60408125,60408126,60408122,60408121,60408120,60408119,2631,1617,641,2642,640,60405020,2653,1655,2647,2648,2649,
                2639,60405028,1805,2645,2646,60405012,599,600,60405029,2603
            };
            //List<int> nodesForWorsening = RastrManager.RayonNodesToList(rastr, 1); //Узлы района 1 (бодайб)
            calculationSettings.NodesForWorsening = RastrManager.RayonNodesToList(rastr, 1).Union(new List<int>() { 1654 }).ToList();
            calculationSettings.PercentForWorsening = 10;
            calculationSettings.PercentLoad = 50;
            List<double> powerFlows=Calculation.CalculatePowerFlows(rastr, calculationSettings);
            Console.WriteLine("Расчет завершен, нажмите любую кнопку");
            Console.ReadKey();
            FileManager.ToExcel(powerFlows, 6, UValueDict);
            //FileManager.ToExcel_I(IValueDict);
        }
    }
}
