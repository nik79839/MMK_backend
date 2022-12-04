namespace Domain.InitialResult
{
    /// <summary>
    /// Результаты расчета в исходном виде
    /// </summary>
    public class CalculationResultInitial
    {
        /// <summary>
        /// Ссылка на результаты расчетов
        /// </summary>
        public List<CalculationResultBase> PowerFlowResults { get; set; } = new();

        /// <summary>
        /// Ссылка на результаты расчетов напряжений
        /// </summary>
        public List<VoltageResult>? VoltageResults { get; set; } = new();

        /// <summary>
        /// Ссылка на результаты расчетов токов
        /// </summary>
        public List<CurrentResult>? CurrentResults { get; set; } = new();

        public List<CalculationResultSet> CalculationResults { get; set; } = new();
    }
}
