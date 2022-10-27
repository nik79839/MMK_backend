namespace Domain.InitialResult
{
    /// <summary>
    /// Результаты расчета
    /// </summary>
    public abstract class CalculationResultBase
    {
        /// <summary>
        /// Уникальный идентификатор расчета
        /// </summary>
        public string CalculationId { get; set; }

        /// <summary>
        /// Номер реализации
        /// </summary>
        public int ImplementationId { get; set; }

        public CalculationResultBase(string calculationId, int implementationId)
        {
            CalculationId = calculationId;
            ImplementationId = implementationId;
        }
    }
}
