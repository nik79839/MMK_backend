using Application.Interfaces;
using Domain;
using Domain.Events;
using Domain.InitialResult;
using Domain.ProcessedResult;
using Domain.Rastrwin3.RastrModel;
using RastrAdapter;
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

        /// <summary>
        /// Удалить расчет по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Получить результаты расчета обработанные и исходные по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public CalculationResultInitial GetCalculationsById(string id)
        {
            CalculationResultInitial calculationResultInitial = _calculationResultRepository.GetResultInitialById(id).Result;
            if (calculationResultInitial.PowerFlowResults.Count == 0)
            {
                throw new Exception($"Ошибка. Расчета с ID {id} не существует.");
            }
            return calculationResultInitial;
        }

        //TODO: События
        public async Task StartCalculation(CalculationSettings calcSettings, CancellationToken cancellationToken)
        {
            RastrCOMClient rastrComClient = new(calcSettings.PathToRegim, calcSettings.PathToSech);
            Calculations calculations = new()
            {
                Name = calcSettings.Name,
                Description = calcSettings.Description,
                PathToRegim = Path.GetFileName(calcSettings.PathToRegim),
                PercentLoad = calcSettings.PercentLoad,
                PercentForWorsening = calcSettings.PercentForWorsening,
                SechName = rastrComClient.SechList().Find(sech => sech.Num == calcSettings.SechNumber).SechName
            };
            Console.WriteLine("Режим и сечения загружены.");
            List<WorseningSettings> worseningSettings = new();
            worseningSettings.AddRange(from setting in calcSettings.WorseningSettings
                                       select new WorseningSettings(calculations.Id, setting.NodeNumber, setting.MaxValue));
            await _calculationResultRepository.AddCalculation(calculations);
            CalculationResultInitial calcResultInit = new();
            List<int> numberLoadNodes = rastrComClient.AllLoadNodesToList().ConvertAll(x => x.Number); //Массив номеров узлов
            int exp = calcSettings.CountOfImplementations;
                                                                  
            for (int i = 0; i < exp; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine("Отмена расчета");
                    await _calculationResultRepository.DeleteCalculationsById(calculations.Id.ToString());
                    return;
                }
                var watch = Stopwatch.StartNew();
                rastrComClient = new(calcSettings.PathToRegim, calcSettings.PathToSech);
                rastrComClient.ChangePn(numberLoadNodes, calcSettings.PercentLoad); //Случайная нагрузка
                rastrComClient.RastrTestBalance();
                rastrComClient.WorseningRandom(calcSettings.WorseningSettings, calcSettings.PercentLoad);
                double powerFlowValue = Math.Round((double)rastrComClient.PowerSech.Z[calcSettings.SechNumber-1], 2);
                calcResultInit.PowerFlowResults.Add(new CalculationResultBase(calculations.Id, i + 1, powerFlowValue));
                calcResultInit.VoltageResults?.AddRange(from int uNode in calcSettings.UNodes // Запись напряжений
                                                       let index = rastrComClient.FindNodeIndex(uNode)
                                                       select new VoltageResult(calculations.Id, i + 1, uNode, rastrComClient.NameNode.Z[index].ToString(), Math.Round(Convert.ToDouble(rastrComClient.Ur.Z[index]), 2)));
                calcResultInit.CurrentResults?.AddRange(from string brunch in calcSettings.IBrunches // Запись токов
                                                       let index = rastrComClient.FindBranchIndexByName(brunch)
                                                       select new CurrentResult(calculations.Id, i + 1, brunch, Math.Round(Convert.ToDouble(rastrComClient.IMax.Z[index]), 2)));
                watch.Stop();
                calculations.Progress = (i + 1) * 100 / exp;
                CalculationProgress.Invoke(this, new CalculationProgressEventArgs(calculations.Id, (int)calculations.Progress, Convert.ToInt32(watch.Elapsed.TotalMinutes * (exp - i + 1)))); //Вызов события
                Console.WriteLine(powerFlowValue);
            }
            await _calculationResultRepository.AddPowerFlowResults(calcResultInit.PowerFlowResults);
            await _calculationResultRepository.AddVoltageResults(calcResultInit.VoltageResults);
            await _calculationResultRepository.AddCurrentResults(calcResultInit.CurrentResults);
            await _calculationResultRepository.UpdateCalculation(calculations);
            await _calculationResultRepository.AddWorseningSettings(worseningSettings);
        }
    }
}
