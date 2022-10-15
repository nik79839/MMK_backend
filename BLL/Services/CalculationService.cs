using AutoMapper;
using BLL.Interfaces;
using Data;
using Data.Entities;
using Data.Entities.Result;
using Data.Events;
using Data.RastrModel;
using Data.Repositories.Abstract;
using Data.Result;
using Data.Statistic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CalculationService : ICalculationService
    {
        private readonly ICalculationResultRepository _calculationResultRepository;
        private readonly IMapper _mapper;
        public event EventHandler<CalculationProgressEventArgs> CalculationProgress;
        public CalculationService(ICalculationResultRepository calculationResultRepository, IMapper mapper)
        {
            _calculationResultRepository = calculationResultRepository;
            _mapper = new MapperConfiguration(cfg => 
            {
                cfg.CreateMap<CalculationEntity, Calculations>().ReverseMap();
                cfg.CreateMap<PowerFlowResultEntity, PowerFlowResult>().ReverseMap();
                cfg.CreateMap<VoltageResultEntity, VoltageResult>().ReverseMap();
                cfg.CreateMap<CurrentResultEntity, CurrentResult>().ReverseMap();
            }).CreateMapper();
        }

        public async Task DeleteCalculationById(string id)
        {
            await _calculationResultRepository.DeleteCalculationsById(id);
        }

        public List<Calculations> GetCalculations()
        {
            return _mapper.Map<List<CalculationEntity>, List<Calculations>>(_calculationResultRepository.GetCalculations().Result);
        }

        public CalculationStatistic GetCalculationsById(string id)
        {
            List<PowerFlowResultEntity> powerFlowResults = _calculationResultRepository.GetPowerFlowResultById(id).Result;
            List<VoltageResultEntity> voltageResults = _calculationResultRepository.GetVoltageResultById(id).Result;
            CalculationStatistic calculationStatistic = new();
            if (powerFlowResults.Count == 0)
            {
                throw new Exception($"Ошибка.Расчета с ID { id } не существует.");
            }
            calculationStatistic.Processing(_mapper.Map<List<PowerFlowResultEntity>, List<PowerFlowResult>>(powerFlowResults));
            calculationStatistic.Processing(_mapper.Map<List<VoltageResultEntity>, List<VoltageResult>>(voltageResults));
            return calculationStatistic;
        }

        //TODO: События
        public async Task StartCalculation(Calculations calculations, CalculationSettings calculationSettings, CancellationToken cancellationToken)
        {
            RastrManager rastrManager = new(calculationSettings.PathToRegim, calculationSettings.PathToSech);
            Console.WriteLine("Режим и сечения загружены.");

            await _calculationResultRepository.AddCalculation(_mapper.Map<Calculations, CalculationEntity>(calculations));
            List<int> nodesWithKP = new() { 2658, 2643, 60408105 };
            List<int> nodesWithSkrm = rastrManager.SkrmNodesToList(); //Заполнение листа с узлами  СКРМ
            List<Brunch> brunchesWithAOPO = new() { new (2640,2641,0), new(2631, 2640, 0), new(2639, 2640, 0),
            new(2639,60408105,0), new (60408105,2630,1)}; // Ветви для замеров тока
            /* if (calculationSettings.IsAllNodesInitial)
             {*/
            calculationSettings.LoadNodes = rastrManager.AllLoadNodesToList(); //Список узлов нагрузки со случайными начальными параметрами (все узлы)
                                                                               //}
            List<int> numberLoadNodes = calculationSettings.LoadNodes.Select(x => x.Number).ToList(); //Массив номеров узлов
            int exp = calculationSettings.CountOfImplementations; // Число реализаций
            calculations.SechName = rastrManager.SechList().Where(sech => sech.Num == calculationSettings.SechNumber).FirstOrDefault().NameSech;
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
                /*RastrRetCode test = rastrManager._Rastr.rgm("p");
                if (test == RastrRetCode.AST_NB)
                {
                    Console.WriteLine($"Итерация {i} не завершена из-за несходимости режима.");
                    continue;
                }*/
                rastrManager.WorseningRandom(calculationSettings.NodesForWorsening, tgNodes, nodesWithKP, calculationSettings.PercentLoad);
                double powerFlowValue = Math.Round((double)rastrManager.PowerSech.Z[calculationSettings.SechNumber], 2);
                for (int j = 0; j < nodesWithKP.Count; j++) // Запись напряжений
                {
                    int index = rastrManager.FindNodeIndex(nodesWithKP[j]);
                    calculations.VoltageResults.Add(new VoltageResult(calculations.Id, i + 1, nodesWithKP[j], rastrManager.NameNode.Z[index].ToString(), Convert.ToDouble(rastrManager.Ur.Z[index])));
                }
                for (int j = 0; j < brunchesWithAOPO.Count; j++) // Запись токов
                {
                    int index = rastrManager.FindBranchIndex(brunchesWithAOPO[j].StartNode, brunchesWithAOPO[j].EndNode, brunchesWithAOPO[j].ParallelNumber);
                    calculations.CurrentResults.Add(new CurrentResult(calculations.Id, i + 1, brunchesWithAOPO[j].StartNode, brunchesWithAOPO[j].EndNode, Convert.ToDouble(rastrManager.IMax.Z[index])));
                }

                watch.Stop();
                calculations.Progress = (i + 1) * 100 / exp;
                //CalculationProgress.Invoke(this, new CalculationProgressEventArgs(calculations.CalculationId, (int)calculations.Progress, Convert.ToInt32(watch.Elapsed.TotalMinutes * (exp - i + 1)))); //Вызов события
                Console.WriteLine(powerFlowValue + " " + i + " Оставшееся время - " + Math.Round(watch.Elapsed.TotalMinutes * (exp - i + 1), 2) + " минут");
                calculations.PowerFlowResults.Add(new PowerFlowResult(calculations.Id, i + 1, powerFlowValue));
            }
            DateTime endTime = DateTime.UtcNow;
            await _calculationResultRepository.AddPowerFlowResults(_mapper.Map<List<PowerFlowResult>, List<PowerFlowResultEntity>>(calculations.PowerFlowResults));
            await _calculationResultRepository.AddVoltageResults(_mapper.Map< List<VoltageResult>, List<VoltageResultEntity>>(calculations.VoltageResults));
            await _calculationResultRepository.UpdateCalculation(_mapper.Map<Calculations, CalculationEntity>(calculations));        
        }
    }
}
