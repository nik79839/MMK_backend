using BLL.Statistic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Result
{
    public class CurrentResult : CalculationResultBase
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
