﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Statistic
{
    /// <summary>
    /// Результаты расчета для хранения в БД
    /// </summary>
    public class HistogramData
    {

        /// <summary>
        /// Диапазон
        /// </summary>
        public string Interval { get; set; }

        /// <summary>
        /// Высота прямоугольника
        /// </summary>
        public double Height { get; set; }


    }
}