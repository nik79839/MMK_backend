using Application.Interfaces;
using Domain;
using Domain.Events;
using Domain.Rastrwin3.RastrModel;
using Domain.Result;
using Domain.Statistic;
using System.Diagnostics;

namespace Infrastructure.Services
{
    public class CalculationService : ICalculationService
    {
        private readonly ICalculationResultRepository _calculationResultRepository;
        public event EventHandler<CalculationProgressEventArgs> CalculationProgress;
        public CalculationService(ICalculationResultRepository calculationResultRepository)
        {
            _calculationResultRepository = calculationResultRepository;
        }

        public async Task DeleteCalculationById(string id)
        {
            await _calculationResultRepository.DeleteCalculationsById(id);
        }

        public List<Calculations> GetCalculations()
        {
            return _calculationResultRepository.GetCalculations().Result;
        }

        public CalculationStatistic GetCalculationsById(string id)
        {
            List<PowerFlowResult> powerFlowResults = _calculationResultRepository.GetPowerFlowResultById(id).Result;
            List<VoltageResult> voltageResults = _calculationResultRepository.GetVoltageResultById(id).Result;
            CalculationStatistic calculationStatistic = new();
            if (powerFlowResults.Count == 0)
            {
                throw new Exception($"Ошибка.Расчета с ID {id} не существует.");
            }
            calculationStatistic.Processing(powerFlowResults);
            calculationStatistic.Processing(voltageResults);
            return calculationStatistic;
        }

        //TODO: События
        public async Task StartCalculation(Calculations calculations, CalculationSettings calculationSettings, CancellationToken cancellationToken)
        {
            RastrProvider rastrProvider = new(calculationSettings.PathToRegim, calculationSettings.PathToSech);
            calculations.SechName = rastrProvider.SechList().FirstOrDefault(sech => sech.Num == calculationSettings.SechNumber).SechName;
            Console.WriteLine("Режим и сечения загружены.");
            await _calculationResultRepository.AddCalculation(calculations);
            List<int> nodesWithKP = new() { 2658, 2643, 60408105 };
            List<int> nodesWithSkrm = rastrProvider.SkrmNodesToList(); //Заполнение листа с узлами  СКРМ
            List<Brunch> brunchesWithAOPO = new() { new (2640,2641,0), new(2631, 2640, 0), new(2639, 2640, 0),
            new(2639,60408105,0), new (60408105,2630,1)}; // Ветви для замеров тока
            calculationSettings.LoadNodes = rastrProvider.AllLoadNodesToList(); //Список узлов нагрузки со случайными начальными параметрами (все узлы)                                                                   //}
            List<int> numberLoadNodes = calculationSettings.LoadNodes.Select(x => x.Number).ToList(); //Массив номеров узлов
            int exp = calculationSettings.CountOfImplementations; // Число реализаций
                                                                  // 
            for (int i = 0; i < exp; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Отмена расчета");
                    break;
                }
                var watch = Stopwatch.StartNew();
                rastrProvider = new(calculationSettings.PathToRegim, calculationSettings.PathToSech);
                //RastrManager.ChangeNodeStateRandom(rastr, nodesWithSkrm); //вкл или выкл для СКРМ 50/50
                List<double> tgNodes = rastrProvider.ChangeTg(numberLoadNodes); //Список коэф мощности для каждой реализации
                rastrProvider.ChangePn(numberLoadNodes, tgNodes, calculationSettings.PercentLoad); //Случайная нагрузка
                rastrProvider.RastrTestBalance();
                rastrProvider.WorseningRandom(calculationSettings.NodesForWorsening, tgNodes, nodesWithKP, calculationSettings.PercentLoad);
                double powerFlowValue = Math.Round((double)rastrProvider.PowerSech.Z[calculationSettings.SechNumber], 2);
                for (int j = 0; j < nodesWithKP.Count; j++) // Запись напряжений
                {
                    int index = rastrProvider.FindNodeIndex(nodesWithKP[j]);
                    calculations.VoltageResults.Add(new VoltageResult(calculations.Id, i + 1, nodesWithKP[j], rastrProvider.NameNode.Z[index].ToString(), Convert.ToDouble(rastrProvider.Ur.Z[index])));
                }
                for (int j = 0; j < brunchesWithAOPO.Count; j++) // Запись токов
                {
                    int index = rastrProvider.FindBranchIndex(brunchesWithAOPO[j].StartNode, brunchesWithAOPO[j].EndNode, brunchesWithAOPO[j].ParallelNumber);
                    calculations.CurrentResults.Add(new CurrentResult(calculations.Id, i + 1, brunchesWithAOPO[j].StartNode, brunchesWithAOPO[j].EndNode, Convert.ToDouble(rastrProvider.IMax.Z[index])));
                }

                watch.Stop();
                calculations.Progress = (i + 1) * 100 / exp;
                CalculationProgress.Invoke(this, new CalculationProgressEventArgs(calculations.Id, (int)calculations.Progress, Convert.ToInt32(watch.Elapsed.TotalMinutes * (exp - i + 1)))); //Вызов события
                Console.WriteLine(powerFlowValue + " " + i + " Оставшееся время - " + Math.Round(watch.Elapsed.TotalMinutes * (exp - i + 1), 2) + " минут");
                calculations.PowerFlowResults.Add(new PowerFlowResult(calculations.Id, i + 1, powerFlowValue));
            }
            DateTime endTime = DateTime.UtcNow;
            await _calculationResultRepository.AddPowerFlowResults(calculations.PowerFlowResults);
            await _calculationResultRepository.AddVoltageResults(calculations.VoltageResults);
            await _calculationResultRepository.UpdateCalculation(calculations);
        }
    }
}
