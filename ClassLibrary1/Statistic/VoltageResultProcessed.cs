using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Statistic
{
    /// <summary>
    /// Результаты расчета обработанные напряжение
    /// </summary>
    public class VoltageResultProcessed: StatisticBase
    {
        /// <summary>
        /// Номер узла
        /// </summary>
        public int NodeNumber { get; set; } = 0;
    }
}
