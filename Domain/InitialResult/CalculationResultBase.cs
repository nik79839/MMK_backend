using System.ComponentModel.DataAnnotations;

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

        [Range(1, 10000, ErrorMessage = "Недопустимое число реализации")]
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
