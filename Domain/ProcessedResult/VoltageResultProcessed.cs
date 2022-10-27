namespace Domain.ProcessedResult
{
    /// <summary>
    /// Результаты расчета обработанные напряжение
    /// </summary>
    public class VoltageResultProcessed : StatisticBase
    {
        /// <summary>
        /// Номер узла
        /// </summary>
        public int NodeNumber { get; set; } = 0;
    }
}
