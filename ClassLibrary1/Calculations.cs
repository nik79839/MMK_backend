using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    /// <summary>
    /// Результаты расчета для хранения в БД
    /// </summary>
    public class Calculations
    {
        /// <summary>
        /// Уникальный идентификатор расчета
        /// </summary>
        [Key, Column(Order = 0)]
        public string CalculationId { get; set; }

        /// <summary>
        /// Название расчета
        /// </summary>
        [Key, Column(Order = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Время начала расчета
        /// </summary>
        public DateTime CalculationStart { get; set; }

        /// <summary>
        /// Время Конца расчета
        /// </summary>
        public DateTime CalculationEnd { get; set; }

        /// <summary>
        /// Ссылка на результаты расчетов
        /// </summary>
        public List<CalculationResult> CalculationResults { get; set; } = new();
    }
}
