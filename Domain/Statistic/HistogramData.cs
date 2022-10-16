namespace Domain.Statistic
{
    /// <summary>
    /// Результаты расчета для хранения в БД
    /// </summary>
    public class HistogramData
    {

        /// <summary>
        /// Диапазон
        /// </summary>
        public string Interval { get; set; }

        /// <summary>
        /// Высота прямоугольника
        /// </summary>
        public double Height { get; set; }


    }
}
