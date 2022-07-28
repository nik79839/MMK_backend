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
    /// Результаты расчета обработанные
    /// </summary>
    public class VoltageResultProcessed
    {

        public int NodeNumber { get; set; }

        /// <summary>
        /// Максимум выборки
        /// </summary>
        public double Maximum { get; set; }

        /// <summary>
        /// Высота прямоугольника
        /// </summary>
        public double Minimum { get; set; }

        /// <summary>
        /// Математическое ожидание
        /// </summary>
        public double Mean { get; set; }

        /// <summary>
        /// Среднеквадратическое отклонение
        /// </summary>
        public double StD { get; set; }

        public List<CalculationResultProcessed> CalculationResultProcesseds { get; set; } = new();
        
    }
}
