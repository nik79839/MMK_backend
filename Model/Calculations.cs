using ASTRALib;
using Model.RastrModel;
using Model.Result;
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
        /// Ссылка на результаты расчетов токов
        /// </summary>
        [NotMapped]
        public List<CurrentResult> CurrentResults { get; set; } = new();

        /// <summary>
        /// Процент прогресса расчета
        /// </summary>
        [NotMapped]
        public int? Progress { get; set; } = null;

        public Rastr _Rastr { get; set; }

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
        public static void WorseningRandom(Rastr rastr, List<int> nodes, List<double> tgvalues, List<int> nodesWithKP,int percent) 
        {
            Random randPercent = new Random();
            ITable node = (ITable)rastr.Tables.Item("node");
            ICol pn = (ICol)node.Cols.Item("pn");
            ICol qn = (ICol)node.Cols.Item("qn");
            ICol sta = (ICol)node.Cols.Item("sta");
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
            }
        }

        /// <summary>
        /// Расчет предельных перетоков для всех реализаций
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="calculationSettings">Объект параметров расчета</param>
        /// <returns>Массив предельных перетоков</returns>
        public void CalculatePowerFlows(CalculationSettings calculationSettings, CancellationToken cancellationToken)
        {
            _Rastr = new();
            _Rastr.Load(RG_KOD.RG_REPL, calculationSettings.PathToRegim, calculationSettings.PathToRegim);
            _Rastr.Load(RG_KOD.RG_REPL, calculationSettings.PathToSech, calculationSettings.PathToSech);
            Console.WriteLine("Режим и сечения загружены.");
            ITable sch = (ITable)_Rastr.Tables.Item("sechen");
            ICol powerSech = (ICol)sch.Cols.Item("psech");
            ITable vetv = (ITable)_Rastr.Tables.Item("vetv");
            ICol vetvName = (ICol)vetv.Cols.Item("name");
            ICol iMax = (ICol)vetv.Cols.Item("i_max");
            ITable node = (ITable)_Rastr.Tables.Item("node");
            ICol Ur = (ICol)node.Cols.Item("vras");
            ICol name = (ICol)node.Cols.Item("name");
            List<int> nodesWithKP = new () { 2658, 2643, 60408105 };
            List<int> nodesWithSkrm = RastrManager.SkrmNodesToList(_Rastr); //Заполнение листа с узлами  СКРМ
            List<Brunch> brunchesWithAOPO = new List<Brunch>() { new Brunch(2640,2641,0), new Brunch(2631, 2640, 0), new Brunch(2639, 2640, 0),
            new Brunch(2639,60408105,0), new Brunch(60408105,2630,1)}; // Ветви для замеров тока
            if (calculationSettings.IsAllNodesInitial)
            {
                calculationSettings.LoadNodes = RastrManager.AllLoadNodesToList(calculationSettings.PathToRegim); //Список узлов нагрузки со случайными начальными параметрами (все узлы)
            }
            List<int> numberLoadNodes = calculationSettings.LoadNodes.Select(x => x.Number).ToList(); //Массив номеров узлов
            calculationSettings.NodesForWorsening = RastrManager.DistrictNodesToList(_Rastr, 1).Union(new List<int>() { 1654 }).ToList();
            int exp = calculationSettings.CountOfImplementations; // Число реализаций
            NameOfSech = RastrManager.SechList(calculationSettings.PathToRegim).Where(sech => sech.Num == calculationSettings.SechNumber).FirstOrDefault().NameSech;
            for (int i = 0; i < exp; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Отмена расчета");
                    break;
                }
                var watch = Stopwatch.StartNew();
                _Rastr.Load(RG_KOD.RG_REPL, calculationSettings.PathToRegim, calculationSettings.PathToRegim);
                //RastrManager.ChangeNodeStateRandom(rastr, nodesWithSkrm); //вкл или выкл для СКРМ 50/50
                List<double> tgNodes = RastrManager.ChangeTg(_Rastr, numberLoadNodes); //Список коэф мощности для каждой реализации
                RastrManager.ChangePn(_Rastr, numberLoadNodes, tgNodes, calculationSettings.PercentLoad); //Случайная нагрузка
                RastrRetCode test = _Rastr.rgm("p");
                if (test == RastrRetCode.AST_NB)
                {
                    Console.WriteLine($"Итерация {i} не завершена из-за несходимости режима.");
                    continue;
                }
                _Rastr.rgm("p");
                WorseningRandom(_Rastr, calculationSettings.NodesForWorsening, tgNodes, nodesWithKP, calculationSettings.PercentLoad);
                double powerFlowValue = Math.Round((double)powerSech.Z[calculationSettings.SechNumber], 2);

                for (int j = 0; j < nodesWithKP.Count; j++) // Запись напряжений
                {
                    int index = RastrManager.FindNodeIndex(_Rastr, nodesWithKP[j]);
                    VoltageResults.Add(new VoltageResult(CalculationId, i + 1, nodesWithKP[j], name.Z[index].ToString(), Convert.ToDouble(Ur.Z[index])));
                }

                for (int j = 0; j < brunchesWithAOPO.Count; j++) // Запись токов
                {
                    int index = RastrManager.FindBranchIndex(_Rastr, brunchesWithAOPO[j].StartNode, brunchesWithAOPO[j].EndNode, brunchesWithAOPO[j].ParallelNumber);
                    CurrentResults.Add(new CurrentResult(CalculationId, i + 1, brunchesWithAOPO[j].StartNode, brunchesWithAOPO[j].EndNode, Convert.ToDouble(iMax.Z[index])));
                }

                watch.Stop();
                Progress = (i + 1) * 100 / exp;
                CalculationProgress.Invoke(this, new CalculationProgressEventArgs(CalculationId,(int)Progress, Convert.ToInt32(watch.Elapsed.TotalMinutes * (exp - i + 1)))); //Вызов события
                Console.WriteLine(powerFlowValue + " " + i + " Оставшееся время - " + Math.Round(watch.Elapsed.TotalMinutes * (exp - i + 1),2) + " минут");
                CalculationResults.Add(new CalculationResult(CalculationId, i + 1, powerFlowValue, VoltageResults));
            }
        }
    }
}
