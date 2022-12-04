using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistance.Entities.Result
{
    [Table("VoltageResult")]
    public class VoltageResultEntity : CalculationResultBaseEntity
    {
        
        /// <summary>
        /// Номер узла
        /// </summary>
        public int NodeNumber { get; set; }

        /// <summary>
        /// Номер реализации
        /// </summary>
        public string? NodeName { get; set; }
    }
}
