using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistance.Entities
{
    [Table("WorseningSettings")]
    public class WorseningSettingsEntity
    {
        public Guid CalculationId { get; set; }
        public int NodeNumber { get; set; }
        public int? MaxValue { get; set; }
        /// <summary>
        /// Ссылка на расчет
        /// </summary>
        public CalculationEntity? Calculation { get; set; }
    }
}
