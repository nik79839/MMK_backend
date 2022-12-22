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
        public IEnumerable<CalculationResultBase> PowerFlowResults { get; set; }

        /// <summary>
        /// Ссылка на результаты расчетов напряжений
        /// </summary>
        public IEnumerable<CalculationResultBase>? VoltageResults { get; set; }

        /// <summary>
        /// Ссылка на результаты расчетов токов
        /// </summary>
        public IEnumerable<CalculationResultBase>? CurrentResults { get; set; }

        public List<CalculationResultSet> CalculationResults { get; set; } = new();
    }
}
