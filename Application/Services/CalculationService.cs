using Application.Interfaces;
using Domain;
using Domain.Enums;
using Domain.Events;
using Domain.InitialResult;
using System.Diagnostics;

namespace Application.Services
{
    public class CalculationService : ICalculationService
    {
        private readonly ICalculationResultRepository _calculationResultRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICalcModel _rastrClient;
        public event EventHandler<CalculationProgressEventArgs> CalculationProgress;
        public CalculationService(ICalculationResultRepository calculationResultRepository, ICalcModel calcModel, IUserRepository userRepository = null)
        {
            _calculationResultRepository = calculationResultRepository;
            _userRepository = userRepository;
            _rastrClient = calcModel;
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
        private Calculations? GetCalculationById(string id)
        {
            return _calculationResultRepository.GetCalculationById(id).Result;
        }

        /// <summary>
        /// Получить результаты расчета обработанные и исходные по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IEnumerable<CalculationResultBase> GetCalculationResultById(string id)
        {
            IEnumerable<CalculationResultBase> calcResultInitial = _calculationResultRepository.GetResultInitialById(id).Result;
            if (calcResultInitial.ToList().Count == 0)
            {
                throw new Exception($"Ошибка. Расчета с ID {id} не существует.");
            }
            return calcResultInitial;
        }

        //TODO: События
        public async Task StartCalculation(CalculationSettings calcSettings, CancellationToken cancellationToken, int? userId = null)
        {
            _rastrClient.CreateInstanceRastr(calcSettings.PathToRegim, calcSettings.PathToSech);
            List<int> numberLoadNodes = _rastrClient.AllNodesToList().Where(x => x.Pn != 0).ToList().ConvertAll(x => x.Number); //Массив номеров узлов
            List<CalculationResultBase> calcResultInitial = new();
            Calculations calculations = new()
            {
                Name = calcSettings.Name,
                Description = calcSettings.Description,
                PathToRegim = Path.GetFileName(calcSettings.PathToRegim),
                PercentLoad = calcSettings.PercentLoad,
                PercentForWorsening = calcSettings.PercentForWorsening,
                SechName = _rastrClient.SechList().Find(sech => sech.Num == calcSettings.SechNumber).SechName,
            };

            calculations.WorseningSettings = (from setting in calcSettings.WorseningSettings
                                              select new WorseningSettings(calculations.Id, setting.NodeNumber, setting.MaxValue)).ToList();
            await _calculationResultRepository.AddCalculation(calculations, userId);

            int exp = calcSettings.CountOfImplementations;
            for (int i = 0; i < exp; i++)
            {
                if (GetCalculationById(calculations.Id.ToString()) == null)
                {
                    throw new Exception("Отмена расчета");
                }
                var watch = Stopwatch.StartNew();
                _rastrClient.CreateInstanceRastr(calcSettings.PathToRegim, calcSettings.PathToSech);
                _rastrClient.ChangePn(numberLoadNodes, calculations.PercentLoad); //Случайная нагрузка
                _rastrClient.RastrTestBalance();
                _rastrClient.WorseningRandom(calcSettings.WorseningSettings, calculations.PercentForWorsening);
                double powerFlowValue = Math.Round(_rastrClient.SechList()[calcSettings.SechNumber - 1].PowerFlow, 2);

                calcResultInitial.Add(new PowerFlowResult(calculations.Id, i + 1, powerFlowValue));
                calcResultInitial.AddRange(_rastrClient.GetResults(calcSettings.UNodes, calculations.Id, i + 1, ParamType.VOLTAGE));
                calcResultInitial.AddRange(_rastrClient.GetResults(calcSettings.IBrunches, calculations.Id, i + 1, ParamType.CURRENT));

                watch.Stop();
                calculations.Progress = (i + 1) * 100 / exp;
                CalculationProgress?.Invoke(this, new CalculationProgressEventArgs(calculations.Id, (int)calculations.Progress,
                    Convert.ToInt32(watch.Elapsed.TotalMinutes * (exp - i + 1)))); //Вызов события
            }

            await _calculationResultRepository.AddCalculationResults(calcResultInitial);
            await _calculationResultRepository.UpdateCalculation(calculations);
        }
    }
}
