using Domain.Rastrwin3.RastrModel;

namespace Domain.Statistic
{
    /// <summary>
    /// Результаты расчета обработанные напряжение
    /// </summary>
    public class CurrentResultProcessed : StatisticBase
    {
        /// <summary>
        /// Номер узла
        /// </summary>
        public Brunch Brunch { get; set; }
    }
}
