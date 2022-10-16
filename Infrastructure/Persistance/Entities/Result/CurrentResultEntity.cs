using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistance.Entities.Result
{
    [Table("CurrentResult")]
    public class CurrentResultEntity : CalculationResultBase
    {
        /// <summary>
        /// Номер реализации
        /// </summary>
        [Key, Column(Order = 2)]
        public int StartNode { get; set; }
        /// <summary>
        /// Номер реализации
        /// </summary>
        [Key, Column(Order = 3)]
        public int EndNode { get; set; }

        /// <summary>
        /// Значение напряжения
        /// </summary>
        public double CurrentValue { get; set; }
    }
}
