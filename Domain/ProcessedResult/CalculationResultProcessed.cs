using Domain.InitialResult;

namespace Domain.ProcessedResult
{
    /// <summary>
    /// Результаты расчета обработанные
    /// </summary>
    public class CalculationResultProcessed
    {
        public StatisticBase PowerFlowResultProcessed { get; set; } = new();

        /// <summary>
        ///  Обработанные значения напряжения
        /// </summary>
        public List<VoltageResultProcessed>? VoltageResultProcessed { get; set; } = new();

        /// <summary>
        ///  Обработанные значения тока
        /// </summary>
        public List<CurrentResultProcessed>? CurrentResultProcessed { get; set; } = new();

        /// <summary>
        /// Обработка результатов расчета
        /// </summary>
        /// <param name="powerFlowResults"></param>
        public void Processing(List<PowerFlowResult> powerFlowResults)
        {
            List<double> values = powerFlowResults.ConvertAll(x => x.PowerFlowLimit);
            PowerFlowResultProcessed = GetStatistic(values);
        }

        /// <summary>
        /// Обработка результатов расчета
        /// </summary>
        /// <param name="calculationResults"></param>
        public void Processing(List<VoltageResult> voltageResults)
        {
            var nodeNumbers = voltageResults.Select(x => x.NodeNumber).ToArray().Distinct();
            foreach (var nodeNumber in nodeNumbers)
            {
                List<double> values = new();
                values.AddRange(from voltageResult in voltageResults
                                where voltageResult.NodeNumber == nodeNumber
                                select voltageResult.VoltageValue);
                StatisticBase statistic = GetStatistic(values);
                VoltageResultProcessed.Add(new VoltageResultProcessed() { Maximum = statistic.Maximum, Minimum = statistic.Minimum, Mean = statistic.Mean, HistogramData = statistic.HistogramData, NodeNumber = nodeNumber });
            }
        }

        public static StatisticBase GetStatistic(List<double> values)
        {
            StatisticBase statisticBase = new();
            int intervalCount = Convert.ToInt32(Math.Log10(values.Count) + Math.Sqrt(values.Count));
            statisticBase.Maximum = values.Max();
            statisticBase.Minimum = values.Min();
            statisticBase.Mean = Math.Round(values.Average(), 2);
            double step = (statisticBase.Maximum - statisticBase.Minimum) / intervalCount;
            double sec = statisticBase.Minimum;
            double first = 0, height = 0;
            for (int i = 0; i < intervalCount; i++)
            {
                int count = 0;
                sec += step;
                first = sec - step;
                count += (from double v in values
                          where v >= first && v <= sec
                          select v).Count();
                height = (Convert.ToDouble(count) / Convert.ToDouble(values.Count))*100;
                statisticBase.HistogramData.Add(new HistogramData() { Interval = Math.Round(first, 2).ToString() + " - " + Math.Round(sec, 2).ToString(), Height = height });
            }
            return statisticBase;
        }

    }
}
