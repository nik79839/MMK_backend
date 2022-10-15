using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [Table("WorseningSettings")]
    public class WorseningSettingsEntity
    {
        public string CalculationId { get; set; }
        public int NodeNumber { get; set; }
        /// <summary>
        /// Ссылка на расчет
        /// </summary>
        public CalculationEntity? Calculation { get; set; }
    }
}
