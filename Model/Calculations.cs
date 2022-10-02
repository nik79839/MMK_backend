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
        public List<PowerFlowResult> PowerFlowResults { get; set; } = new();

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

        /// <summary>
        /// Расчет предельных перетоков для всех реализаций
        /// </summary>
        /// <param name="rastr"></param>
        /// <param name="calculationSettings">Объект параметров расчета</param>
        /// <returns>Массив предельных перетоков</returns>
        public void CalculatePowerFlows(CalculationSettings calculationSettings, CancellationToken cancellationToken)
        {
            RastrManager rastrManager = new(calculationSettings.PathToRegim, calculationSettings.PathToSech);
            Console.WriteLine("Режим и сечения загружены.");
            List<int> nodesWithKP = new () { 2658, 2643, 60408105 };
            List<int> nodesWithSkrm = rastrManager.SkrmNodesToList(); //Заполнение листа с узлами  СКРМ
            List<Brunch> brunchesWithAOPO = new() { new (2640,2641,0), new(2631, 2640, 0), new(2639, 2640, 0),
            new(2639,60408105,0), new (60408105,2630,1)}; // Ветви для замеров тока
           /* if (calculationSettings.IsAllNodesInitial)
            {*/
                calculationSettings.LoadNodes = rastrManager.AllLoadNodesToList(); //Список узлов нагрузки со случайными начальными параметрами (все узлы)
            //}
            List<int> numberLoadNodes = calculationSettings.LoadNodes.Select(x => x.Number).ToList(); //Массив номеров узлов
            int exp = calculationSettings.CountOfImplementations; // Число реализаций
            NameOfSech = rastrManager.SechList().Where(sech => sech.Num == calculationSettings.SechNumber).FirstOrDefault().NameSech;
            for (int i = 0; i < exp; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Отмена расчета");
                    break;
                }
                var watch = Stopwatch.StartNew();
                rastrManager = new(calculationSettings.PathToRegim, calculationSettings.PathToSech);
                //RastrManager.ChangeNodeStateRandom(rastr, nodesWithSkrm); //вкл или выкл для СКРМ 50/50
                List<double> tgNodes = rastrManager.ChangeTg(numberLoadNodes); //Список коэф мощности для каждой реализации
                rastrManager.ChangePn(numberLoadNodes, tgNodes, calculationSettings.PercentLoad); //Случайная нагрузка
                RastrRetCode test = rastrManager._Rastr.rgm("p");
                if (test == RastrRetCode.AST_NB)
                {
                    Console.WriteLine($"Итерация {i} не завершена из-за несходимости режима.");
                    continue;
                }
                rastrManager.WorseningRandom(calculationSettings.NodesForWorsening, tgNodes, nodesWithKP, calculationSettings.PercentLoad);
                double powerFlowValue = Math.Round((double)rastrManager.PowerSech.Z[calculationSettings.SechNumber], 2);
                for (int j = 0; j < nodesWithKP.Count; j++) // Запись напряжений
                {
                    int index = rastrManager.FindNodeIndex(nodesWithKP[j]);
                    VoltageResults.Add(new VoltageResult(CalculationId, i + 1, nodesWithKP[j], rastrManager.NameNode.Z[index].ToString(), Convert.ToDouble(rastrManager.Ur.Z[index])));
                }
                for (int j = 0; j < brunchesWithAOPO.Count; j++) // Запись токов
                {
                    int index = rastrManager.FindBranchIndex(brunchesWithAOPO[j].StartNode, brunchesWithAOPO[j].EndNode, brunchesWithAOPO[j].ParallelNumber);
                    CurrentResults.Add(new CurrentResult(CalculationId, i + 1, brunchesWithAOPO[j].StartNode, brunchesWithAOPO[j].EndNode, Convert.ToDouble(rastrManager.IMax.Z[index])));
                }

                watch.Stop();
                Progress = (i + 1) * 100 / exp;
                CalculationProgress.Invoke(this, new CalculationProgressEventArgs(CalculationId,(int)Progress, Convert.ToInt32(watch.Elapsed.TotalMinutes * (exp - i + 1)))); //Вызов события
                Console.WriteLine(powerFlowValue + " " + i + " Оставшееся время - " + Math.Round(watch.Elapsed.TotalMinutes * (exp - i + 1),2) + " минут");
                PowerFlowResults.Add(new PowerFlowResult(CalculationId, i + 1, powerFlowValue));
            }
        }
    }
}
