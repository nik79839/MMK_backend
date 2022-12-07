using Domain.InitialResult;

namespace Domain.ProcessedResult
{
    /// <summary>
    /// Результаты расчета обработанные
    /// </summary>
    public class CalculationResultProcessed
    {
        public StatisticBase PowerFlowResultProcessed { get; set; } = new();

        /// <summary>
        ///  Обработанные значения напряжения для нескольких узлов
        /// </summary>
        public List<VoltageResultProcessed>? VoltageResultProcessed { get; set; } = new();

        /// <summary>
        ///  Обработанные значения тока для нескольких ветвей
        /// </summary>
        public List<CurrentResultProcessed>? CurrentResultProcessed { get; set; } = new();
    }
}
