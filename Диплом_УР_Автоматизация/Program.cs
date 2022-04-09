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
            string rg2Path = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\Экспер2.rg2";
            string ut2Path = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\Экспер.ut2";
            rastr.Load(RG_KOD.RG_REPL, rg2Path, RastrTemplate.rg2);
            Console.WriteLine("Режим загружен.");
            rastr.Load(RG_KOD.RG_REPL, ut2Path, RastrTemplate.ut2);
            Console.WriteLine("Траектория утяжеления загружена.");
            List<int> NagrNodes = new List<int>() //Список узлов нагрузки
            {
                1654, 2654, 2652,
                2649, 2647,2646
            };
            List<int> NagrValue = new List<int>() //Список максимумов нагрузки
            {
                80, 92, 112,
                139, 259,1411
            };
            List<double> tgNodes = new List<double>(); //Список коэф мощности для каждой реализации
            List<List<double>> Unodes = new List<List<double>>(); // Список напряжений для каждого узла каждой реализации
            List<double> powerFlow = new List<double>(); // Список значений Pпред
            List<List<double>> data = new List<List<double>>(); //Список реализаций факторов
            List<List<int>> factors = new List<List<int>>(); //Список списков значений факторов (всех)
            int exp = 300; // Число реализаций
            for (int i = 0; i < exp; i++)
            {
                rastr.Load(RG_KOD.RG_REPL, rg2Path, RastrTemplate.rg2);
                ASTRALib.ITable node = (ITable)rastr.Tables.Item("node");
                ICol P = (ICol)node.Cols.Item("pg");
                ICol Pn = (ICol)node.Cols.Item("pn");
                ICol Qn = (ICol)node.Cols.Item("qn");
                RastrFunc.ChangeTg(rastr, NagrNodes, tgNodes);
                ChangePn(rastr, NagrNodes, tgNodes);
                rastr.rgm("p");
                //RastrFunc.Worsening(rastr, ut2Path);
                WorseningRandom(rastr, NagrNodes, tgNodes,Unodes,30);
                Console.WriteLine(Math.Round(Convert.ToDouble(P.Z[7]), 2));
                powerFlow.Add(Convert.ToDouble(P.Z[7]));
            }
            ToExcel(powerFlow,Unodes, 8);
        }
        public static void WorseningRandom(Rastr rastr, List<int> nodes, List<double> tgvalues, List<List<double>> unode,int ex) // Осуществляет процедуру утяжеления, увеличивая нагрузку по процентам
        {
            List<double> U = new List<double>();
            Random randPercent = new Random();
            rastr.rgm("p");
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol pn = (ICol)node.Cols.Item("pn");
            ICol qn = (ICol)node.Cols.Item("qn");
            ICol Ur = (ICol)node.Cols.Item("vras");
            RastrRetCode kod = rastr.rgm("p");
            int index;
                if (kod == 0)
                {
                    RastrRetCode kd;
                    do
                    {
                        kd = rastr.rgm("p");
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = RastrFunc.FindNodeIndex(rastr, nodes[i]);
                        pn.set_ZN(index, Convert.ToDouble(pn.Z[index])* (1+(float)randPercent.Next(0,30)/100));
                        qn.set_ZN(index, Convert.ToDouble(pn.ZN[index]) * tgvalues[i]);
                    }
                    kd = rastr.rgm("p");
                }
                    while (kd == 0);
                for (int i = 0; i < nodes.Count; i++)
                {
                    index = RastrFunc.FindNodeIndex(rastr, nodes[i]);
                    U.Add(Convert.ToDouble(Ur.Z[index]));
                }
                unode.Add(U);
                }
        }
        public static void ToExcel(List<double> powerFlow, List<List<double>> U, int list) // Запись в Excel мощностей и напряжений
        {
            string path = @"C:\Users\otrok\Desktop\Дипломмаг\Экспер.xlsm";
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
