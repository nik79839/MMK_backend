using Domain.Rastrwin3.RastrModel;

namespace Domain.ProcessedResult
{
    /// <summary>
    /// Результаты расчета обработанные напряжение
    /// </summary>
    public class CurrentResultProcessed : StatisticBase
    {
        /// <summary>
        /// Номер узла
        /// </summary>
        public string BrunchName { get; set; }
    }
}
