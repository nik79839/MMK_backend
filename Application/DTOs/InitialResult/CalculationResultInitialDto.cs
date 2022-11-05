namespace Application.DTOs.InitialResult
{
    /// <summary>
    /// Результаты расчета в исходном виде
    /// </summary>
    public class CalculationResultInitialDto
    {
        /// <summary>
        /// Ссылка на результаты расчетов
        /// </summary>
        public List<double> PowerFlowResults { get; set; } = new();

        /// <summary>
        /// Ссылка на результаты расчетов напряжений
        /// </summary>
        public List<VoltageResultDto>? VoltageResults { get; set; } = new();

        /// <summary>
        /// Ссылка на результаты расчетов токов
        /// </summary>
        public List<CurrentResultDto>? CurrentResults { get; set; } = new();
    }
}
