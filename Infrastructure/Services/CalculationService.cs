using Application.Interfaces;
using Domain;
using Domain.Events;
using Domain.InitialResult;
using Domain.ProcessedResult;
using Domain.Rastrwin3.RastrModel;
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

        public CalculationResultInfo GetCalculationsById(string id)
        {
            CalculationResultInitial calculationResultInitial = new()
            {
                PowerFlowResults = _calculationResultRepository.GetPowerFlowResultById(id).Result,
                VoltageResults = _calculationResultRepository.GetVoltageResultById(id).Result
            };
            
            if (calculationResultInitial.PowerFlowResults.Count == 0)
            {
                throw new Exception($"Ошибка. Расчета с ID {id} не существует.");
            }

            CalculationResultProcessed calculationResultProcessed = new();
            calculationResultProcessed.Processing(calculationResultInitial.PowerFlowResults);
            calculationResultProcessed.Processing(calculationResultInitial.VoltageResults);
            List<WorseningSettings> worseningSettings = _calculationResultRepository.GetWorseningSettingsById(id).Result;
            List<int> worseningSettingsNumber = worseningSettings.Select(x => x.NodeNumber).ToList();
            CalculationResultInfo calculationResultInfo = new(calculationResultInitial, calculationResultProcessed, worseningSettingsNumber);
            return calculationResultInfo;
        }

        //TODO: События
        public async Task StartCalculation(Calculations calculations, CalculationSettings calculationSettings, CancellationToken cancellationToken)
        {
            RastrProvider rastrProvider = new(calculationSettings.PathToRegim, calculationSettings.PathToSech);
            Console.WriteLine("Режим и сечения загружены.");
            calculations.SechName = rastrProvider.SechList().FirstOrDefault(sech => sech.Num == calculationSettings.SechNumber).SechName;
            List<WorseningSettings> worseningSettings = new();
            foreach (var setting in calculationSettings.NodesForWorsening)
            {
                worseningSettings.Add(new WorseningSettings() { CalculationId = calculations.Id, NodeNumber = setting });
            }
            await _calculationResultRepository.AddCalculation(calculations);
            await _calculationResultRepository.AddWorseningSettings(worseningSettings);
            CalculationResultInitial calculationResultInitial = new();
            List<int> nodesWithKP = new() { 2658, 2643, 60408105 };
            List<Brunch> brunchesWithAOPO = new() { new (2640,2641,0), new(2631, 2640, 0), new(2639, 2640, 0),
            new(2639,60408105,0), new (60408105,2630,1)}; // Ветви для замеров тока
            calculationSettings.LoadNodes = rastrProvider.AllLoadNodesToList(); //Список узлов нагрузки со случайными начальными параметрами (все узлы)                                                                   //}
            List<int> numberLoadNodes = calculationSettings.LoadNodes.Select(x => x.Number).ToList(); //Массив номеров узлов
            int exp = calculationSettings.CountOfImplementations; // Число реализаций
                                                                  
            for (int i = 0; i < exp; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Отмена расчета");
                    break;
                }
                var watch = Stopwatch.StartNew();
                rastrProvider = new(calculationSettings.PathToRegim, calculationSettings.PathToSech);
                List<double> tgNodes = rastrProvider.ChangeTg(numberLoadNodes); //Список коэф мощности для каждой реализации
                rastrProvider.ChangePn(numberLoadNodes, tgNodes, calculationSettings.PercentLoad); //Случайная нагрузка
                rastrProvider.RastrTestBalance();
                rastrProvider.WorseningRandom(calculationSettings.NodesForWorsening, tgNodes, nodesWithKP, calculationSettings.PercentLoad);
                double powerFlowValue = Math.Round((double)rastrProvider.PowerSech.Z[calculationSettings.SechNumber], 2);
                for (int j = 0; j < nodesWithKP.Count; j++) // Запись напряжений
                {
                    int index = rastrProvider.FindNodeIndex(nodesWithKP[j]);
                    calculationResultInitial.VoltageResults.Add(new VoltageResult(calculations.Id, i + 1, nodesWithKP[j], rastrProvider.NameNode.Z[index].ToString(), Convert.ToDouble(rastrProvider.Ur.Z[index])));
                }
                for (int j = 0; j < brunchesWithAOPO.Count; j++) // Запись токов
                {
                    int index = rastrProvider.FindBranchIndex(brunchesWithAOPO[j].StartNode, brunchesWithAOPO[j].EndNode, brunchesWithAOPO[j].ParallelNumber);
                    calculationResultInitial.CurrentResults.Add(new CurrentResult(calculations.Id, i + 1, brunchesWithAOPO[j].StartNode, brunchesWithAOPO[j].EndNode, Convert.ToDouble(rastrProvider.IMax.Z[index])));
                }

                watch.Stop();
                calculations.Progress = (i + 1) * 100 / exp;
                CalculationProgress.Invoke(this, new CalculationProgressEventArgs(calculations.Id, (int)calculations.Progress, Convert.ToInt32(watch.Elapsed.TotalMinutes * (exp - i + 1)))); //Вызов события
                Console.WriteLine(powerFlowValue + " " + i + " Оставшееся время - " + Math.Round(watch.Elapsed.TotalMinutes * (exp - i + 1), 2) + " минут");
                calculationResultInitial.PowerFlowResults.Add(new PowerFlowResult(calculations.Id, i + 1, powerFlowValue));
            }
            DateTime endTime = DateTime.UtcNow;
            await _calculationResultRepository.AddPowerFlowResults(calculationResultInitial.PowerFlowResults);
            await _calculationResultRepository.AddVoltageResults(calculationResultInitial.VoltageResults);
            await _calculationResultRepository.UpdateCalculation(calculations);
        }
    }
}
