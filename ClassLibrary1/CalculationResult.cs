﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    /// <summary>
    /// Результаты расчета
    /// </summary>
    public class CalculationResult
    {
        /// <summary>
        /// Уникальный идентификатор расчета
        /// </summary>
        [Key, Column(Order = 0)]
        public int CalculationId { get; set; }

        /// <summary>
        /// Номер реализации
        /// </summary>
        [Key, Column(Order = 1)]
        public int ImplementationId { get; set; }

        /// <summary>
        /// Предельный переток
        /// </summary>
        public double PowerFlowLimit { get; set; }
    }
}
