using Domain.Rastrwin3.RastrModel;

namespace Application.DTOs.ProcessedResult
{
    /// <summary>
    /// Результаты расчета обработанные напряжение
    /// </summary>
    public class CurrentResultProcessedDto : StatisticBaseDto
    {
        /// <summary>
        /// Номер узла
        /// </summary>
        public Brunch Brunch { get; set; }
    }
}
