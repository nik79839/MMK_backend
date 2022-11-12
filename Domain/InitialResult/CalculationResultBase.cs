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
        public Guid CalculationId { get; set; }

        /// <summary>
        /// Номер реализации
        /// </summary>
        public int ImplementationId { get; set; }

        public CalculationResultBase(Guid calculationId, int implementationId)
        {
            CalculationId = calculationId;
            ImplementationId = implementationId;
        }
    }
}
