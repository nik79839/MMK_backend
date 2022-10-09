﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BLL.RastrModel;

namespace BLL.Statistic
{
    /// <summary>
    /// Результаты расчета обработанные напряжение
    /// </summary>
    public class CurrentResultProcessed: StatisticBase
    {
        /// <summary>
        /// Номер узла
        /// </summary>
        public Brunch Brunch { get; set; }
    }
}