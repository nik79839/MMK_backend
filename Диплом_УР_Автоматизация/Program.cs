using System;
using System.Collections.Generic;
using ASTRALib;
using Excel = Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using ForRastr;
using System.Linq;

namespace Диплом_УР_Автоматизация
{
    public static class RastrTemplate
    {
        public const string rg2 = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\Экспер2.rg2";
        public const string ut2 = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\Экспер.ut2";
        public const string sch = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\СБЭК_сечения.sch";
    }
    class Program
    {
        static void Main(string[] args)
        {
            Rastr rastr = new Rastr();
            Random rand = new Random();
            string rg2Path = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\СБЭК_СХН.rg2";
            string ut2Path = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\СБЭК.ut2";
            string schPath = @"C:\Users\otrok\Desktop\Дипломмаг\Мой\СБЭК_сечения.sch";
            rastr.Load(RG_KOD.RG_REPL, rg2Path, RastrTemplate.rg2);
            Console.WriteLine("Режим загружен.");
            rastr.Load(RG_KOD.RG_REPL, ut2Path, RastrTemplate.ut2);
            Console.WriteLine("Траектория утяжеления загружена.");
            rastr.Load(RG_KOD.RG_REPL, schPath, RastrTemplate.sch);
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
            //List<int> nodesForWorsening = RastrFunc.RayonNodesToList(rastr, 1); //Узлы района 1 (бодайб)
            List<int> nodesForWorsening = RastrFunc.RayonNodesToList(rastr, 1).Union(RastrFunc.RayonNodesToList(rastr, 2)).ToList();
            List<double> tgNodes = new List<double>(); //Список коэф мощности для каждой реализации
            List<List<double>> Unodes = new List<List<double>>(); // Список величин напряжений для каждого узла каждой реализации
            List<List<double>> Ibranches = new List<List<double>>(); // Список величин токов для каждого узла каждой реализации
            List<double> powerFlow = new List<double>(); // Список значений Pпред
            List<List<double>> data = new List<List<double>>(); //Список реализаций факторов
            List<List<int>> factors = new List<List<int>>(); //Список списков значений факторов (всех)
            List<int> nodesWithKP = new List<int>() {2658,2643, 60408105 };
            List<int> nodesWithSkrm=new List<int>();
            Dictionary<string, List<double>> UValueDict = new Dictionary<string, List<double>>(); //Словарь со значениями напряжений
            Dictionary<string, List<double>> IValueDict = new Dictionary<string, List<double>>(); //Словарь со значениями токов
            RastrFunc.SkrmNodesToList(rastr, nodesWithSkrm); //Заполнение листа с узлами  СКРМ
            List<int> brunchesWithAOPO = new List<int>() { RastrFunc.FindBranchIndex(rastr, 2640, 2641, 0), RastrFunc.FindBranchIndex(rastr, 2631, 2640, 0),
                RastrFunc.FindBranchIndex(rastr, 2639, 2640, 0),RastrFunc.FindBranchIndex(rastr, 2639, 60408105, 0), RastrFunc.FindBranchIndex(rastr, 60408105, 2630, 1),
                RastrFunc.FindBranchIndex(rastr, 2620, 60408105, 0), RastrFunc.FindBranchIndex(rastr, 2610, 60408105, 0)}; // Индексы
            
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol name = (ICol)node.Cols.Item("name");
            ITable vetv = (ITable)rastr.Tables.Item("vetv");
            ICol vetvName = (ICol)vetv.Cols.Item("name");
            for (int i = 0; i < nodesWithKP.Count; i++) // Создание ключей в словаре
            {
                int index = RastrFunc.FindNodeIndex(rastr, nodesWithKP[i]);
                UValueDict.Add(name.Z[index].ToString(), new List<double>());
            }
            foreach (var index in brunchesWithAOPO) IValueDict.Add(vetvName.Z[index].ToString(), new List<double>()); // Создание ключей в словаре

            int exp = 100; // Число реализаций
            for (int i = 0; i < exp; i++)
            {
                rastr.Load(RG_KOD.RG_REPL, rg2Path, RastrTemplate.rg2);
                //RastrFunc.ChangeNodeStateRandom(rastr, nodesWithSkrm); //вкл или выкл для СКРМ 50/50
                RastrFunc.ChangeTg(rastr, NagrNodes, tgNodes);
                ChangePn(rastr, NagrNodes, tgNodes);
                RastrRetCode test= rastr.rgm("p");
                if (test == RastrRetCode.AST_NB)
                {
                    Console.WriteLine($"Итерация {i} не завершена из-за несходимости режима.");
                    continue;
                }
                rastr.rgm("p");
                //RastrFunc.Worsening(rastr, ut2Path);
                double powerFlowValue = WorseningRandom(rastr, nodesForWorsening, tgNodes, nodesWithKP,brunchesWithAOPO,UValueDict,IValueDict);
                //double powerFlowSech = Math.Round(Convert.ToDouble(powerSech.Z[1]),2);
                Console.WriteLine(powerFlowValue + " "+i);
                powerFlow.Add(powerFlowValue);
            }
            //ToExcel(powerFlow, 3,UValueDict);
            ToExcel_I(IValueDict);
        }
        public static double WorseningRandom(Rastr rastr, List<int> nodes, List<double> tgvalues, List<int> nodesWithKP,
            List<int> brunchesWithAOPO, Dictionary<string,List<double>> UValueDict, Dictionary<string,List<double>> IValueDict) // Осуществляет процедуру утяжеления, увеличивая нагрузку по процентам
        {         
            Random randPercent = new Random();
            ITable node = (ITable)rastr.Tables.Item("node");
            ITable sch = (ITable)rastr.Tables.Item("sechen");
            ICol powerSech = (ICol)sch.Cols.Item("psech");
            ICol pn = (ICol)node.Cols.Item("pn");
            ICol qn = (ICol)node.Cols.Item("qn");
            ICol Ur = (ICol)node.Cols.Item("vras");
            ICol sta = (ICol)node.Cols.Item("sta");
            ICol name = (ICol)node.Cols.Item("name");
            ITable vetv = (ITable)rastr.Tables.Item("vetv");
            ICol vetvName = (ICol)vetv.Cols.Item("name");
            ICol iMax = (ICol)vetv.Cols.Item("i_max");
            RastrRetCode kod = rastr.rgm("p");
            float randomPercent;
            int index;
            
            if (kod == 0)
            {
                RastrRetCode kd;
                do
                {                  
                    for (int i = 0; i < nodes.Count; i++) //Основное утяжеление
                    {
                        index = RastrFunc.FindNodeIndex(rastr, nodes[i]);
                        randomPercent = 1 + (float)randPercent.Next(0, 15) / 100;
                        pn.set_ZN(index, Convert.ToDouble(pn.Z[index]) * randomPercent);
                        qn.set_ZN(index, Convert.ToDouble(pn.ZN[index]) * tgvalues[i]);
                    }
                    kd = rastr.rgm("p");
                    /*for (int i = 0; i < nodesWithKP.Count; i++) // Изменение СКРМ
                    {
                        index = RastrFunc.FindNodeIndex(rastr, nodesWithKP[i]);
                        if (Convert.ToDouble(Ur.Z[index]) < 225)
                        {
                            Console.WriteLine("Включение БСК");
                            index = RastrFunc.FindNodeIndex(rastr, 60408136);
                            sta.set_ZN(index, 0);
                            RastrFunc.ConnectedBranchState(rastr, 60408136, 2, 0);
                        }
                    }
                    kd = rastr.rgm("p");*/                   
                }
                while (kd == 0);
                while (kd != 0) // Откат на последний сходяийся режим
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = RastrFunc.FindNodeIndex(rastr, nodes[i]);
                        pn.set_ZN(index, Convert.ToDouble(pn.Z[index]) / 1.02);
                        qn.set_ZN(index, Convert.ToDouble(pn.ZN[index]) * tgvalues[i]);
                    }
                    kd = rastr.rgm("p");                  
                }
                for (int i = 0; i < nodesWithKP.Count; i++) // Запись напряжений
                {
                    index = RastrFunc.FindNodeIndex(rastr, nodesWithKP[i]);
                    UValueDict[name.Z[index].ToString()].Add(Convert.ToDouble(Ur.Z[index]));
                }
                for (int i = 0; i < brunchesWithAOPO.Count; i++) // Запись токов в ветвях с АОПО
                {
                    IValueDict[vetvName.Z[brunchesWithAOPO[i]].ToString()].Add(Convert.ToDouble(iMax.Z[brunchesWithAOPO[i]]));
                }
            }
            return Math.Round(Convert.ToDouble(powerSech.Z[1]), 2);
        }
        public static void ToExcel(List<double> powerFlow, int list, Dictionary<string, List<double>> UValueDict) // Запись в Excel мощностей и напряжений
        {
            string path = @"C:\Users\otrok\Desktop\Дипломмаг\СБЭКv2.xlsx";
            using (var excel = new ExcelPackage(new System.IO.FileInfo(path)))
            {
                var ws = excel.Workbook.Worksheets[list];
                for (int k = 0; k < powerFlow.Count; k++)
                {
                    ws.Cells[k + 2, 1].Value = powerFlow[k];
                }
                int column = 15; // Initialize for keys.
                foreach (string key in UValueDict.Keys)
                {
                    int row = 1; // Initialize for values in key.
                    ws.Cells[row, column].Value = key;
                    foreach (var value in UValueDict[key])
                    {
                        row++;
                        ws.Cells[row, column].Value = value;
                    }
                    column++; // increment for next key.
                }
                excel.Save();
            }
        }

        public static void ToExcel_I(Dictionary<string, List<double>> IValueDict) // Запись в Excel мощностей и напряжений
        {
            using (var excel = new ExcelPackage())
            {
                var ws = excel.Workbook.Worksheets.Add("Лист1");
                int column = 1; // Initialize for keys.
                foreach (string key in IValueDict.Keys)
                {
                    int row = 1; // Initialize for values in key.
                    ws.Cells[row, column].Value = key;
                    foreach (var value in IValueDict[key])
                    {
                        row++;
                        ws.Cells[row, column].Value = Math.Round(value*1000,2);
                    }
                    column++; // increment for next key.
                }
                excel.SaveAs(new System.IO.FileInfo(@"C:\Users\otrok\Desktop\Дипломмаг\Тест_I.xlsx")) ;
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
