using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ASTRALib;

namespace Диплом_УР_Автоматизация
{
    class Program
    {
        static void Main(string[] args)
        {
            Rastr rastr = new Rastr();
            string rg2Path = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\СБЭК_СХН.rg2";
            string ut2Path = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\СБЭК.ut2";
            string schPath = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\СБЭК_сечения.sch";
            rastr.Load(RG_KOD.RG_REPL, rg2Path, rg2Path);
            Console.WriteLine("Режим загружен.");
            rastr.Load(RG_KOD.RG_REPL, ut2Path, ut2Path);
            Console.WriteLine("Траектория утяжеления загружена.");
            rastr.Load(RG_KOD.RG_REPL, schPath, schPath);
            Console.WriteLine("Сечения загружены.");
            ITable sch = (ITable)rastr.Tables.Item("sechen");
            ICol powerSech = (ICol)sch.Cols.Item("psech");
            List<int> NagrNodes = new List<int>() //Список узлов нагрузки
            {
                1620, 1610, 2643,
                2605, 1800,1654,1619,60408134,60408133,2630,60405013,1618,1616,2652,60405014,2644,60408115,60408116,2641,60408123,
                60408124,60408125,60408126,60408122,60408121,60408120,60408119,2631,1617,641,2642,640,60405020,2653,1655,2647,2648,2649,
                2639,60405028,1805,2645,2646,60405012,599,600,60405029,2603
            };
            //List<int> nodesForWorsening = RastrManager.RayonNodesToList(rastr, 1); //Узлы района 1 (бодайб)
            List<int> nodesForWorsening = RastrManager.RayonNodesToList(rastr, 1).Union(new List<int>() { 1654}).ToList();
            List<double> tgNodes = new List<double>(); //Список коэф мощности для каждой реализации
            List<double> powerFlow = new List<double>(); // Список значений Pпред
            List<int> nodesWithKP = new List<int>() {2658,2643, 60408105 };
            List<int> nodesWithSkrm=new List<int>();
            Dictionary<string, List<double>> UValueDict = new Dictionary<string, List<double>>(); //Словарь со значениями напряжений
            Dictionary<string, List<double>> IValueDict = new Dictionary<string, List<double>>(); //Словарь со значениями токов
            RastrManager.SkrmNodesToList(rastr, nodesWithSkrm); //Заполнение листа с узлами  СКРМ
            List<int> brunchesWithAOPO = new List<int>() { RastrManager.FindBranchIndex(rastr, 2640, 2641, 0), RastrManager.FindBranchIndex(rastr, 2631, 2640, 0),
                RastrManager.FindBranchIndex(rastr, 2639, 2640, 0),RastrManager.FindBranchIndex(rastr, 2639, 60408105, 0), RastrManager.FindBranchIndex(rastr, 60408105, 2630, 1),}; // Индексы
            
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol name = (ICol)node.Cols.Item("name");
            ITable vetv = (ITable)rastr.Tables.Item("vetv");
            ICol vetvName = (ICol)vetv.Cols.Item("name");
            for (int i = 0; i < nodesWithKP.Count; i++) // Создание ключей в словаре
            {
                int index = RastrManager.FindNodeIndex(rastr, nodesWithKP[i]);
                UValueDict.Add(name.Z[index].ToString(), new List<double>());
            }
            foreach (var index in brunchesWithAOPO) IValueDict.Add(vetvName.Z[index].ToString(), new List<double>()); // Создание ключей в словаре

            int exp = 100; // Число реализаций
            for (int i = 0; i < exp; i++)
            {
                var watch = Stopwatch.StartNew();
                rastr.Load(RG_KOD.RG_REPL, rg2Path, rg2Path);
                //RastrManager.ChangeNodeStateRandom(rastr, nodesWithSkrm); //вкл или выкл для СКРМ 50/50
                RastrManager.ChangeTg(rastr, NagrNodes, tgNodes);
                RastrManager.ChangePn(rastr, NagrNodes, tgNodes);
                RastrRetCode test= rastr.rgm("p");
                if (test == RastrRetCode.AST_NB)
                {
                    Console.WriteLine($"Итерация {i} не завершена из-за несходимости режима.");
                    continue;
                }
                rastr.rgm("p");
                //Calculation.Worsening(rastr, ut2Path);
                double powerFlowValue = Calculation.WorseningRandom(rastr, nodesForWorsening, tgNodes, nodesWithKP,brunchesWithAOPO,UValueDict,IValueDict);
                //double powerFlowValue = Math.Round(Convert.ToDouble(powerSech.Z[1]),2);
                watch.Stop();
                Console.WriteLine(powerFlowValue + " "+i+" Оставшееся время - " + watch.Elapsed.TotalMinutes*(exp-i+1)+" минут");
                powerFlow.Add(powerFlowValue);
            }
            Console.WriteLine("Расчет завершен, нажмите любую кнопку");
            Console.ReadKey();
            FileManager.ToExcel(powerFlow, 6,UValueDict);
            //FileManager.ToExcel_I(IValueDict);
        }        
    }
}
