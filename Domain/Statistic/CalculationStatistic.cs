using Domain.Result;

namespace Domain.Statistic
{
    /// <summary>
    /// Результаты расчета обработанные
    /// </summary>
    public class CalculationStatistic
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
            List<double> values = powerFlowResults.Select(x => x.PowerFlowLimit).ToList();
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
                foreach (var voltageResult in voltageResults)
                {
                    if (voltageResult.NodeNumber == nodeNumber)
                    {
                        values.Add(voltageResult.VoltageValue);
                    }
                }
                int intervalCount = Convert.ToInt32(Math.Log10(values.Count) + Math.Sqrt(values.Count));
                double maximum = values.Max();
                double minimum = values.Min();
                double mean = Math.Round(values.Average(), 2);
                double range = maximum - minimum;
                double step = range / intervalCount;
                double sec = minimum;
                double first = 0, height = 0;
                List<HistogramData> histogramData = new();
                for (int i = 0; i < intervalCount; i++)
                {
                    int count = 0;
                    sec = sec + step;
                    first = sec - step;
                    for (int j = 0; j < values.Count; j++)
                    {
                        if (values[j] >= first && values[j] <= sec)
                        {
                            count++;
                        }
                    }
                    height = Convert.ToDouble(count) / Convert.ToDouble(values.Count) / step;
                    histogramData.Add(new HistogramData() { Interval = Math.Round(first, 2).ToString() + " - " + Math.Round(sec, 2).ToString(), Height = height });
                }
                VoltageResultProcessed.Add(new VoltageResultProcessed() { Maximum = maximum, Minimum = minimum, Mean = mean, HistogramData = histogramData, NodeNumber = nodeNumber });
            }
        }

        public static StatisticBase GetStatistic(List<double> values)
        {
            StatisticBase statisticBase = new();
            int intervalCount = Convert.ToInt32(Math.Log10(values.Count) + Math.Sqrt(values.Count));
            statisticBase.Maximum = values.Max();
            statisticBase.Minimum = values.Min();
            statisticBase.Mean = Math.Round(values.Average(), 2);
            double range = statisticBase.Maximum - statisticBase.Minimum;
            double step = range / intervalCount;
            double sec = statisticBase.Minimum;
            double first = 0, height = 0;
            for (int i = 0; i < intervalCount; i++)
            {
                int count = 0;
                sec = sec + step;
                first = sec - step;
                for (int j = 0; j < values.Count; j++)
                {
                    if (values[j] >= first && values[j] <= sec)
                    {
                        count++;
                    }
                }
                height = Convert.ToDouble(count) / Convert.ToDouble(values.Count) / step;
                statisticBase.HistogramData.Add(new HistogramData() { Interval = Math.Round(first, 2).ToString() + " - " + Math.Round(sec, 2).ToString(), Height = height });
            }
            return statisticBase;
        }

    }
}
