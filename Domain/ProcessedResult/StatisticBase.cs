namespace Domain.ProcessedResult
{
    public class StatisticBase
    {
        /// <summary>
        /// Максимум выборки
        /// </summary>
        public double Maximum { get; set; }

        /// <summary>
        /// Высота прямоугольника
        /// </summary>
        public double Minimum { get; set; }

        /// <summary>
        /// Математическое ожидание
        /// </summary>
        public double Mean { get; set; }

        /// <summary>
        /// Среднеквадратическое отклонение
        /// </summary>
        public double StD { get; set; }

        /// <summary>
        /// Столбцы гистограммы
        /// </summary>
        public List<HistogramData> HistogramData { get; set; }

        public void GetStatistic(List<double> values)
        {
            int intervalCount = Convert.ToInt32(Math.Log10(values.Count) + Math.Sqrt(values.Count));
            Maximum = values.Max();
            Minimum = values.Min();
            Mean = Math.Round(values.Average(), 2);
            double dispersion = values.Sum(a => (a - Mean) * (a - Mean)) / (values.Count - 1);
            StD = Math.Sqrt(dispersion);
            double step = (Maximum - Minimum) / intervalCount;
            double sec = Minimum;
            double first = 0, height = 0;
            for (int i = 0; i < intervalCount; i++)
            {
                int count = 0;
                sec += step;
                first = sec - step;
                count += (from double v in values
                          where v >= first && v <= sec
                          select v).Count();
                height = Math.Round((Convert.ToDouble(count) / Convert.ToDouble(values.Count)) * 100, 2);
                HistogramData.Add(new HistogramData() { Interval = $"{Math.Round(first, 2)} - {Math.Round(sec, 2)}", Height = height });
            }
        }
    }
}
