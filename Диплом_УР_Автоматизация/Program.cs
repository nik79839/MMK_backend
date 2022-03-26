using System;
using System.Collections.Generic;
using ASTRALib;
using Excel = Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using ForRastr;

namespace Диплом_УР_Автоматизация
{
    public static class RastrTemplate
    {
        public const string rg2 = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\Экспер2.rg2";
        public const string ut2 = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\Экспер.ut2";
    }
    class Program
    {
        static void Main(string[] args)
        {
            Rastr rastr = new Rastr();
            Random rand = new Random();
            string rg2Path = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\СБЭК_СХН.rg2";
            string ut2Path = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\СБЭК.ut2";
            rastr.Load(RG_KOD.RG_REPL, rg2Path, RastrTemplate.rg2);
            Console.WriteLine("Режим загружен.");
            rastr.Load(RG_KOD.RG_REPL, ut2Path, RastrTemplate.ut2);
            Console.WriteLine("Траектория утяжеления загружена.");
            List<int> NagrNodes = new List<int>() //Список узлов нагрузки
            {
                1620, 1610, 2643,
                2605, 1800,1654,1619,60408134,60408133,2630,60405013,1618,1616,2652,60405014,2644,60408115,60408116,2641,60408123,
                60408124,60408125,60408126,60408122,60408121,60408120,60408119,2631,1617,641,2642,640,60405020,2653,1655,2647,2648,2649,
                2639,60405028,1805,2645,2646,60405012,599,600,60405029,2603
            };
            List<int> nodesForWorsening = RastrFunc.RayonNodesToList(rastr, 1); //Узлы района 1 (бодайб)
            List<double> tgNodes = new List<double>(); //Список коэф мощности для каждой реализации
            List<List<double>> Unodes = new List<List<double>>(); // Список напряжений для каждого узла каждой реализации
            List<double> powerFlow = new List<double>(); // Список значений Pпред
            List<List<double>> data = new List<List<double>>(); //Список реализаций факторов
            List<List<int>> factors = new List<List<int>>(); //Список списков значений факторов (всех)
            List<int> nodesWithKP = new List<int>() {2658 };
            List<int> nodesWithSkrm=new List<int>();
            RastrFunc.SkrmNodesToList(rastr, nodesWithSkrm); //Заполнение листа с узлами  СКРМ
            int exp = 100; // Число реализаций
            for (int i = 0; i < exp; i++)
            {
                rastr.Load(RG_KOD.RG_REPL, rg2Path, RastrTemplate.rg2);
                ASTRALib.ITable node = (ITable)rastr.Tables.Item("node");
                ASTRALib.ITable vetv = (ITable)rastr.Tables.Item("vetv");
                ICol P = (ICol)node.Cols.Item("pg");
                ICol Pn = (ICol)node.Cols.Item("pn");
                ICol Qn = (ICol)node.Cols.Item("qn");
                ICol powerStart = (ICol)vetv.Cols.Item("pl_ip");
                //RastrFunc.ChangeNodeStateRandom(rastr, nodesWithSkrm); //вкл или выкл для СКРМ 50/50
                RastrFunc.ChangeTg(rastr, NagrNodes, tgNodes);
                ChangePn(rastr, NagrNodes, tgNodes);
                RastrRetCode test= rastr.rgm("p");
                if (test == RastrRetCode.AST_NB)
                {
                    Console.WriteLine($"Итерация {i} не завершена из-за несходимости режима.");
                    //rastr.Save($"разошелся {i}.rg2", "");
                    // Вроде проблема решена
                    continue;
                }
                rastr.rgm("p");
                //RastrFunc.Worsening(rastr, ut2Path);
                double powerFlowValue = WorseningRandom(rastr, nodesForWorsening, tgNodes, Unodes, nodesWithKP);
                //double powerFlowSech = -Math.Round(Convert.ToDouble(powerStart.Z[RastrFunc.FindBranchIndex(rastr, 2654, 2658, 0)]) +
                //    Convert.ToDouble(powerStart.Z[RastrFunc.FindBranchIndex(rastr, 2654, 1656, 0)]), 2);
                Console.WriteLine(powerFlowValue + " "+i);
                powerFlow.Add(powerFlowValue);
            }
            ToExcel(powerFlow,Unodes, 4);
        }
        public static double WorseningRandom(Rastr rastr, List<int> nodes, List<double> tgvalues, List<List<double>> unode, List<int> nodesWithKP) // Осуществляет процедуру утяжеления, увеличивая нагрузку по процентам
        {
            List<double> U = new List<double>();
            List<double> UStep = new List<double>();
            List<double> powerFlowStep = new List<double>();
            Random randPercent = new Random();
            rastr.rgm("p");
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol pn = (ICol)node.Cols.Item("pn");
            ICol qn = (ICol)node.Cols.Item("qn");
            ICol Ur = (ICol)node.Cols.Item("vras");
            ASTRALib.ITable vetv = (ITable)rastr.Tables.Item("vetv");
            ICol powerStart = (ICol)vetv.Cols.Item("pl_ip");
            RastrRetCode kod = rastr.rgm("p");
            float randomPercent = 0;
            int index;
            if (kod == 0)
            {
                RastrRetCode kd;
                do
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = RastrFunc.FindNodeIndex(rastr, nodes[i]);
                        randomPercent = 1 + (float)randPercent.Next(0, 15) / 100;
                        pn.set_ZN(index, Convert.ToDouble(pn.Z[index]) * randomPercent);
                        qn.set_ZN(index, Convert.ToDouble(pn.ZN[index]) * tgvalues[i]);
                    }
                    kd = rastr.rgm("p");
                    powerFlowStep.Add(-Math.Round(Convert.ToDouble(powerStart.Z[RastrFunc.FindBranchIndex(rastr, 2654, 2658, 0)]) +
                    Convert.ToDouble(powerStart.Z[RastrFunc.FindBranchIndex(rastr, 2654, 1656, 0)]), 2));
                    //Console.WriteLine(-Math.Round(Convert.ToDouble(powerStart.Z[RastrFunc.FindBranchIndex(rastr, 2654, 2658, 0)]) +
                    //Convert.ToDouble(powerStart.Z[RastrFunc.FindBranchIndex(rastr, 2654, 1656, 0)]), 2));
                }
                while (kd == 0);
                //int j = 1;
                while (kd != 0)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = RastrFunc.FindNodeIndex(rastr, nodes[i]);
                        pn.set_ZN(index, Convert.ToDouble(pn.Z[index]) / 1.02);
                        qn.set_ZN(index, Convert.ToDouble(pn.ZN[index]) * tgvalues[i]);
                    }
                    kd = rastr.rgm("p");
                    //j++;
                }
                for (int i = 0; i < nodesWithKP.Count; i++)
                {
                    index = RastrFunc.FindNodeIndex(rastr, nodesWithKP[i]);
                    U.Add(Convert.ToDouble(Ur.Z[index]));
                }
                unode.Add(U);         
            }
            return powerFlowStep[powerFlowStep.Count - 2];
        }
        public static void ToExcel(List<double> powerFlow, List<List<double>> U, int list) // Запись в Excel мощностей и напряжений
        {
            string path = @"C:\Users\otrok\Pictures\СБЭК.xlsx";
            using (var excel = new ExcelPackage(new System.IO.FileInfo(path)))
            {
                var ws = excel.Workbook.Worksheets[list];
                for (int k = 0; k < powerFlow.Count; k++)
                {
                    ws.Cells[k + 2, 1].Value = powerFlow[k];
                }
                for (int i=0;i < U.Count; i++) // Реализации
                {
                    for (int k=0; k < U[i].Count; k++) // Узлы
                    {
                        ws.Cells[i+2, k+19].Value = U[i][k];
                    }
                }
                excel.Save();
            }
        }

        public static void ChangePn(Rastr rastr, List<int> nodes, List<double> tgValues) //Случайная нагрузка для каждой реализации
        {
            Random randPn = new Random();
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol pn  = (ICol)node.Cols.Item("pn");
            ICol qn = (ICol)node.Cols.Item("qn");
            RastrRetCode kod = rastr.rgm("p");
                for (int i = 0; i < nodes.Count; i++)
                {
                    int index = RastrFunc.FindNodeIndex(rastr, nodes[i]);
                    pn.set_ZN(index, (float)randPn.Next(Convert.ToInt32(pn.ZN[index]) * 10 / 2, Convert.ToInt32(pn.ZN[index]) * 15) / 10f);
                    qn.set_ZN(index, Convert.ToDouble(pn.ZN[index]) * tgValues[i]);
                }
        }

        
    }
}
