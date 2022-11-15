using Domain.InitialResult;

namespace Application.DTOs.ProcessedResult
{
    /// <summary>
    /// Результаты расчета обработанные
    /// </summary>
    public class CalculationResultProcessedDto
    {
        public StatisticBaseDto PowerFlowResultProcessed { get; set; } = new();

        /// <summary>
        ///  Обработанные значения напряжения
        /// </summary>
        public List<VoltageResultProcessedDto>? VoltageResultProcessed { get; set; } = new();

        /// <summary>
        ///  Обработанные значения тока
        /// </summary>
        public List<CurrentResultProcessedDto>? CurrentResultProcessed { get; set; } = new();
    }
}
