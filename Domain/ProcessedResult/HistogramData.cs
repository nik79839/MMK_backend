namespace Domain.ProcessedResult
{
    /// <summary>
    /// Данные для построения гистограммы плотности вероятности
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
