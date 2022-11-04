using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistance.Entities.Result
{
    /// <summary>
    /// Результаты расчета для хранения в БД
    /// </summary>
    public abstract class CalculationResultBase
    {
        /// <summary>
        /// Уникальный идентификатор расчета
        /// </summary>
        [Key, Column(Order = 0)]
        public Guid CalculationId { get; set; }

        /// <summary>
        /// Номер реализации
        /// </summary>
        [Key, Column(Order = 1)]
        public int ImplementationId { get; set; }

        /// <summary>
        /// Ссылка на расчет
        /// </summary>
        public CalculationEntity? Calculation { get; set; }

    }
}
