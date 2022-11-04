namespace Application.DTOs.ProcessedResult
{
    /// <summary>
    /// Результаты расчета обработанные напряжение
    /// </summary>
    public class VoltageResultProcessedDto : StatisticBaseDto
    {
        /// <summary>
        /// Номер узла
        /// </summary>
        public int NodeNumber { get; set; } = 0;
    }
}
