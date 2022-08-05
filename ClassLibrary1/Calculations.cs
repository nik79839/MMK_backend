using ASTRALib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Calculations
    {
        /// <summary>
        /// Событие описания прогресса расчета
        /// </summary>
        public event EventHandler<CalculationProgressEventArgs> CalculationProgress;

        /// <summary>
        /// Уникальный идентификатор расчета
        /// </summary>
        [Key, Column(Order = 0)]
        public string CalculationId { get; set; }

        /// <summary>
        /// Название расчета
        /// </summary>
        [Key, Column(Order = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Время начала расчета
        /// </summary>
        public DateTime CalculationStart { get; set; }

        /// <summary>
        /// Время Конца расчета
        /// </summary>
        public DateTime? CalculationEnd { get; set; }

        public string? NameOfSech { get; set; }

        /// <summary>
        /// Ссылка на результаты расчетов
        /// </summary>
        public List<CalculationResult> CalculationResults { get; set; } = new();

        /// <summary>
        /// Ссылка на результаты расчетов напряжений
        /// </summary>
        public List<VoltageResult> VoltageResults { get; set; } = new();

        /// <summary>
        /// Процент прогресса расчета
        /// </summary>
        [NotMapped]
        public int? Progress { get; set; } = null;

        /// <summary>
        /// Утяжеление по заданной траектории
        /// </summary>
        /// <param name="rastr">объект растра</param>
        /// <param name="ut2Path">Путь к файлу утяжеления в формате ut2</param>
        public static void Worsening(Rastr rastr, string ut2Path) // Осуществляет процедуру утяжеления.
        {
            // Первая строчка: Параметры/Утяжеление/Формировать описание... (вроде в траектории хранится)
            rastr.Load(RG_KOD.RG_REPL, ut2Path, ut2Path);
            if (rastr.ut_Param[ParamUt.UT_FORM_P] == 0)
            {
                rastr.ClearControl();
                RastrRetCode kod = rastr.step_ut("i");
                if (kod == 0)
                {
                    RastrRetCode kd;
                    do
                    {
                        kd = rastr.step_ut("z");
                        if (((kd == 0) && (rastr.ut_Param[ParamUt.UT_ADD_P] == 0)) || rastr.ut_Param[ParamUt.UT_TIP] == 1)
                        {
                            rastr.AddControl(-1, "");
                        }
                    }
                    while (kd == 0);
                }
            }
        }

        /// <summary>
        /// Случайное утяжеление, утяжеляет нагрузки на случайный процент до расхождения режима
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="nodes">Узлы утяжеления</param>
        /// <param name="tgvalues">Список cos f, генерируется в другом методе случайным образом</param>
        /// <param name="unode">Список напряжений </param>
        /// <param name="percent">Процент приращения</param>
        public static void WorseningRandom(Rastr rastr, List<int> nodes, List<double> tgvalues, List<int> nodesWithKP,
            List<int> brunchesWithAOPO, Dictionary<string, List<double>> IValueDict, int percent) 
        {
            Random randPercent = new Random();
            ITable node = (ITable)rastr.Tables.Item("node");
            ITable sch = (ITable)rastr.Tables.Item("sechen");
            ICol powerSech = (ICol)sch.Cols.Item("psech");
            ICol pn = (ICol)node.Cols.Item("pn");
            ICol qn = (ICol)node.Cols.Item("qn");
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
                do // Основное утяжеление
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = RastrManager.FindNodeIndex(rastr, nodes[i]);
                        randomPercent = 1 + (float)randPercent.Next(0, percent) / 100;
                        pn.set_ZN(index, (double)Convert.ToDouble(pn.Z[index]) * randomPercent);
                        qn.set_ZN(index, (double)Convert.ToDouble(pn.ZN[index]) * tgvalues[i]);
                    }
                    kd = rastr.rgm("p");
                    /*for (int i = 0; i < nodesWithKP.Count; i++) // Изменение СКРМ
                    {
                        index = RastrManager.FindNodeIndex(rastr, nodesWithKP[i]);
                        if (Convert.ToDouble(Ur.Z[index]) < 225)
                        {
                            Console.WriteLine("Включение БСК");
                            index = RastrManager.FindNodeIndex(rastr, 60408136);
                            sta.set_ZN(index, 0);
                            RastrManager.ConnectedBranchState(rastr, 60408136, 2, 0);
                        }
                    }
                    kd = rastr.rgm("p");*/
                }
                while (kd == 0);
                while (kd != 0) // Откат на последний сходяийся режим
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        index = RastrManager.FindNodeIndex(rastr, nodes[i]);
                        pn.set_ZN(index, (double)pn.Z[index] / 1.02);
                        qn.set_ZN(index, (double)pn.ZN[index] * tgvalues[i]);
                    }
                    kd = rastr.rgm("p");
                }
                for (int i = 0; i < brunchesWithAOPO.Count; i++) // Запись токов в ветвях с АОПО
                {
                    IValueDict[vetvName.Z[brunchesWithAOPO[i]].ToString()].Add(Convert.ToDouble(iMax.Z[brunchesWithAOPO[i]]));
                }
            }
            //return Math.Round((double)powerSech.Z[1], 2);
        }

        /// <summary>
        /// Расчет предельных перетоков для всех реализаций
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="calculationSettings">Объект параметров расчета</param>
        /// <returns>Массив предельных перетоков</returns>
        public void CalculatePowerFlows(Rastr rastr, CalculationSettings calculationSettings, CancellationToken cancellationToken)
        {
            Console.WriteLine("Режим загружен.");
            //rastr.Load(RG_KOD.RG_REPL, ut2Path, ut2Path);
            //Console.WriteLine("Траектория утяжеления загружена.");
            rastr.Load(RG_KOD.RG_REPL, calculationSettings.PathToRegim, calculationSettings.PathToRegim);
            rastr.Load(RG_KOD.RG_REPL, calculationSettings.PathToSech, calculationSettings.PathToSech);
            Console.WriteLine("Сечения загружены.");
            ITable sch = (ITable)rastr.Tables.Item("sechen");
            ICol powerSech = (ICol)sch.Cols.Item("psech");          
            List<double> tgNodes = new List<double>(); //Список коэф мощности для каждой реализации
            List<int> nodesWithKP = new List<int>() { 2658, 2643, 60408105 };
            List<int> nodesWithSkrm = RastrManager.SkrmNodesToList(rastr); //Заполнение листа с узлами  СКРМ
            Dictionary<string, List<double>> IValueDict = new Dictionary<string, List<double>>(); //Словарь со значениями токов
            List<int> brunchesWithAOPO = new List<int>() { RastrManager.FindBranchIndex(rastr, 2640, 2641, 0), RastrManager.FindBranchIndex(rastr, 2631, 2640, 0),
                RastrManager.FindBranchIndex(rastr, 2639, 2640, 0),RastrManager.FindBranchIndex(rastr, 2639, 60408105, 0), RastrManager.FindBranchIndex(rastr, 60408105, 2630, 1),}; // Индексы

            
            ITable vetv = (ITable)rastr.Tables.Item("vetv");
            ICol vetvName = (ICol)vetv.Cols.Item("name");
            foreach (var index in brunchesWithAOPO) IValueDict.Add(vetvName.Z[index].ToString(), new List<double>()); // Создание ключей в словаре

            int exp = calculationSettings.CountOfImplementations; // Число реализаций
            for (int i = 0; i < exp; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Отмена расчета");
                    break;
                }
                var watch = Stopwatch.StartNew();
                rastr.Load(RG_KOD.RG_REPL, calculationSettings.PathToRegim, calculationSettings.PathToRegim);
                ITable node = (ITable)rastr.Tables.Item("node");
                ICol Ur = (ICol)node.Cols.Item("vras");
                ICol name = (ICol)node.Cols.Item("name");
                //RastrManager.ChangeNodeStateRandom(rastr, nodesWithSkrm); //вкл или выкл для СКРМ 50/50
                RastrManager.ChangeTg(rastr, calculationSettings.LoadNodes, tgNodes);
                RastrManager.ChangePn(rastr, calculationSettings.LoadNodes, tgNodes, calculationSettings.PercentLoad);
                RastrRetCode test = rastr.rgm("p");
                if (test == RastrRetCode.AST_NB)
                {
                    Console.WriteLine($"Итерация {i} не завершена из-за несходимости режима.");
                    continue;
                }
                rastr.rgm("p");
                //Calculation.Worsening(rastr, ut2Path);
                WorseningRandom(rastr, calculationSettings.NodesForWorsening, tgNodes, nodesWithKP, brunchesWithAOPO, IValueDict, calculationSettings.PercentLoad);
                double powerFlowValue = Math.Round((double)powerSech.Z[calculationSettings.SechNumber], 2);

                for (int j = 0; j < nodesWithKP.Count; j++) // Запись напряжений
                {
                    int index = RastrManager.FindNodeIndex(rastr, nodesWithKP[j]);
                    VoltageResults.Add(new VoltageResult() { NodeName = name.Z[index].ToString(), NodeNumber = nodesWithKP[j], 
                        VoltageValue = Convert.ToDouble(Ur.Z[index]),CalculationId= this.CalculationId, ImplementationId=i+1 });
                }

                //double powerFlowValue = Math.Round(Convert.ToDouble(powerSech.Z[1]),2);
                watch.Stop();
                Progress = (i + 1) * 100 / exp;
                CalculationProgress.Invoke(this, new CalculationProgressEventArgs(CalculationId,(int)Progress, Convert.ToInt32(watch.Elapsed.TotalMinutes * (exp - i + 1)))); //Вызов события
                Console.WriteLine(powerFlowValue + " " + i + " Оставшееся время - " + Math.Round(watch.Elapsed.TotalMinutes * (exp - i + 1),2) + " минут");
                CalculationResults.Add(new CalculationResult() { ImplementationId = i+1, PowerFlowLimit = powerFlowValue,CalculationId = this.CalculationId,VoltageResult=VoltageResults });
                //foreach (VoltageResult voltageResult in voltageResults) VoltageResults.Add()
                //VoltageResults.Add
            }
        }
    }
}
