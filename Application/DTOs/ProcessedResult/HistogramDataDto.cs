namespace Application.DTOs.ProcessedResult
{
    /// <summary>
    /// Результаты расчета для хранения в БД
    /// </summary>
    public class HistogramDataDto
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
