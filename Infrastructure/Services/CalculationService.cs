using Application.Interfaces;
using Domain;
using Domain.Events;
using Domain.InitialResult;
using Domain.ProcessedResult;
using Domain.Rastrwin3.RastrModel;
using System.Diagnostics;
using System.Linq;

namespace Infrastructure.Services
{
    public class CalculationService : ICalculationService
    {
        private readonly ICalculationResultRepository _calculationResultRepository;
        private readonly ICalcModel _calcModel;
        public event EventHandler<CalculationProgressEventArgs> CalculationProgress;
        public CalculationService(ICalculationResultRepository calculationResultRepository, ICalcModel calcModel)
        {
            _calculationResultRepository = calculationResultRepository;
            _calcModel = calcModel;
        }

        public async Task DeleteCalculationById(string id)
        {
            await _calculationResultRepository.DeleteCalculationsById(id);
        }

        /// <summary>
        /// Получить все расчеты
        /// </summary>
        /// <returns>Расчеты</returns>
        public List<Calculations> GetCalculations()
        {
            return _calculationResultRepository.GetCalculations().Result;
        }

        public CalculationResultInfo GetCalculationsById(string id)
        {
            CalculationResultInitial calculationResultInitial = _calculationResultRepository.GetResultInitialById(id).Result;
            if (calculationResultInitial.PowerFlowResults.Count == 0)
            {
                throw new Exception($"Ошибка. Расчета с ID {id} не существует.");
            }

            CalculationResultProcessed calculationResultProcessed = new();
            calculationResultProcessed.Processing(calculationResultInitial.PowerFlowResults);
            calculationResultProcessed.Processing(calculationResultInitial.VoltageResults);
            CalculationResultInfo calculationResultInfo = new(calculationResultInitial, calculationResultProcessed);
            return calculationResultInfo;
        }

        //TODO: События
        public async Task StartCalculation(CalculationSettings calculationSettings, CancellationToken cancellationToken)
        {
            RastrProvider rastrProvider = new(calculationSettings.PathToRegim, calculationSettings.PathToSech);
            Calculations calculations = new()
            {
                Name = calculationSettings.Name,
                Description = calculationSettings.Description,
                CalculationEnd = null,
                PathToRegim = calculationSettings.PathToRegim,
                PercentLoad = calculationSettings.PercentLoad,
                PercentForWorsening = calculationSettings.PercentForWorsening,
                SechName = rastrProvider.SechList().FirstOrDefault(sech => sech.Num == calculationSettings.SechNumber).SechName
            };
            Console.WriteLine("Режим и сечения загружены.");
            List<WorseningSettings> worseningSettings = new();
            worseningSettings.AddRange(from setting in calculationSettings.NodesForWorsening
                                       select new WorseningSettings(calculations.Id, setting));
            await _calculationResultRepository.AddCalculation(calculations);
            await _calculationResultRepository.AddWorseningSettings(worseningSettings);
            CalculationResultInitial calculationResultInitial = new();
            List<int> nodesWithKP = new() { 2658, 2643, 60408105 };
            List<Brunch> brunchesWithAOPO = new() { new (2640,2641,0), new(2631, 2640, 0), new(2639, 2640, 0)}; // Ветви для замеров тока
            calculationSettings.LoadNodes = rastrProvider.AllLoadNodesToList(); //Список узлов нагрузки со случайными начальными параметрами(все узлы)                                                                   //}
            List<int> numberLoadNodes = calculationSettings.LoadNodes.Select(x => x.Number).ToList(); //Массив номеров узлов
            int exp = calculationSettings.CountOfImplementations;
                                                                  
            for (int i = 0; i < exp; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Отмена расчета");
                    break;
                }
                var watch = Stopwatch.StartNew();
                rastrProvider = new(calculationSettings.PathToRegim, calculationSettings.PathToSech);
                rastrProvider.ChangePn(numberLoadNodes, calculationSettings.PercentLoad); //Случайная нагрузка
                rastrProvider.RastrTestBalance();
                rastrProvider.WorseningRandom(calculationSettings.NodesForWorsening, calculationSettings.PercentLoad);
                double powerFlowValue = Math.Round((double)rastrProvider.PowerSech.Z[calculationSettings.SechNumber], 2);
                foreach (int nodeWithKP in nodesWithKP) // Запись напряжений
                {
                    int index = rastrProvider.FindNodeIndex(nodeWithKP);
                    calculationResultInitial.VoltageResults.Add(new VoltageResult(calculations.Id, i + 1, nodeWithKP, rastrProvider.NameNode.Z[index].ToString(), Math.Round(Convert.ToDouble(rastrProvider.Ur.Z[index]),2)));
                }
                for (int j = 0; j < brunchesWithAOPO.Count; j++) // Запись токов
                {
                    int index = rastrProvider.FindBranchIndex(brunchesWithAOPO[j].StartNode, brunchesWithAOPO[j].EndNode, brunchesWithAOPO[j].ParallelNumber);
                    calculationResultInitial.CurrentResults.Add(new CurrentResult(calculations.Id, i + 1, brunchesWithAOPO[j].StartNode, brunchesWithAOPO[j].EndNode, Math.Round(Convert.ToDouble(rastrProvider.IMax.Z[index]),2)));
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
