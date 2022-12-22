using Application.DTOs.InitialResult;
using Application.DTOs.ProcessedResult;

namespace Application.DTOs.Response
{
    /// <summary>
    /// Результат расчета, отправляемый клиенту
    /// </summary>
    public class CalculationResultInfoResponse
    {
                /// <summary>
        /// Ссылка на результаты расчетов
        /// </summary>
        public List<PowerFlowResultDto> PowerFlowResults { get; set; } = new();

        /// <summary>
        /// Ссылка на результаты расчетов напряжений
        /// </summary>
        public List<VoltageResultDto>? VoltageResults { get; set; } = new();

        /// <summary>
        /// Ссылка на результаты расчетов токов
        /// </summary>
        public List<CurrentResultDto>? CurrentResults { get; set; } = new();
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
