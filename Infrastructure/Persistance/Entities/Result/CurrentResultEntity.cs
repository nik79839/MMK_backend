using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistance.Entities.Result
{
    [Table("CurrentResult")]
    public class CurrentResultEntity : CalculationResultBaseEntity
    {
        /// <summary>
        /// Номер реализации
        /// </summary>
        public string BrunchName { get; set; }

        /// <summary>
        /// Значение напряжения
        /// </summary>
        public double CurrentValue { get; set; }
    }
}
