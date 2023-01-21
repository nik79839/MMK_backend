using Application.Interfaces;
using Domain.InitialResult;
using Domain.ProcessedResult;

namespace Application.Services
{
    public class ProcessResultService : IProcessResultService
    {
        /// <summary>
        /// Обработка результатов расчета
        /// </summary>
        /// <param name="powerFlowResults"></param>
        public PowerFlowResultProcessed Processing(List<PowerFlowResult> powerFlowResults)
        {
            List<double> values = powerFlowResults.ConvertAll(x => x.Value);
            PowerFlowResultProcessed statisticBase = new();
            statisticBase.GetStatistic(values);
            return statisticBase;
        }

        /// <summary>
        /// Обработка результатов расчета
        /// </summary>
        /// <param name="calculationResults"></param>
        public IEnumerable<VoltageResultProcessed> Processing(List<VoltageResult> voltageResults)
        {
            List<VoltageResultProcessed> voltageResultProcessed = new();
            var nodeNumbers = voltageResults.Select(x => x.NodeNumber).ToArray().Distinct();
            foreach (var nodeNumber in nodeNumbers)
            {
                List<double> values = new();
                string nodeName = voltageResults.First(x => x.NodeNumber == nodeNumber).NodeName;
                values.AddRange(from voltageResult in voltageResults
                                where voltageResult.NodeNumber == nodeNumber
                                select voltageResult.Value);
                VoltageResultProcessed statisticBase = new();
                statisticBase.GetStatistic(values);
                statisticBase.NodeName = nodeName;
                voltageResultProcessed.Add(statisticBase);
            }
            return voltageResultProcessed;
        }

        public IEnumerable<CurrentResultProcessed> Processing(List<CurrentResult> currentResults)
        {
            List<CurrentResultProcessed> currentResultProcesseds = new();
            var brunchNames = currentResults.Select(x => x.BrunchName).ToArray().Distinct();
            foreach (var brunchName in brunchNames)
            {
                List<double> values = new();
                values.AddRange(from currentResult in currentResults
                                where currentResult.BrunchName == brunchName
                                select currentResult.Value);
                CurrentResultProcessed statisticBase = new();
                statisticBase.GetStatistic(values);
                statisticBase.BrunchName = brunchName;
                currentResultProcesseds.Add(statisticBase);
            }
            return currentResultProcesseds;
        }

        /*public IEnumerable<StatisticBase> Processing(IEnumerable<CalculationResultBase> calculationResults)
        {
            List<StatisticBase> currentResultProcesseds = new();
            var brunchNames = currentResults.Select(x => x.BrunchName).ToArray().Distinct();
            foreach (var brunchName in brunchNames)
            {
                List<double> values = new();
                values.AddRange(from currentResult in currentResults
                                where currentResult.BrunchName == brunchName
                                select currentResult.Value);
                CurrentResultProcessed statisticBase = new CurrentResultProcessed();
                statisticBase.GetStatistic(values);
                statisticBase.BrunchName = brunchName;
                currentResultProcesseds.Add(statisticBase);
            }
            return currentResultProcesseds;
        }*/
    }
}
