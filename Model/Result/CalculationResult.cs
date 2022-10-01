﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Result
{
    /// <summary>
    /// Результаты расчета для хранения в БД
    /// </summary>
    public class CalculationResult
    {
        /// <summary>
        /// Уникальный идентификатор расчета
        /// </summary>
        [Key, Column(Order = 0)]
        public string CalculationId { get; set; }

        /// <summary>
        /// Номер реализации
        /// </summary>
        [Key, Column(Order = 1)]
        public int ImplementationId { get; set; }

        /// <summary>
        /// Предельный переток
        /// </summary>
        public double PowerFlowLimit { get; set; }

        /// <summary>
        /// Ссылка на расчет
        /// </summary>
        public Calculations? Calculations { get; set; }

        /// <summary>
        /// Напряжение
        /// </summary>
        [NotMapped]
        public List<VoltageResult>? VoltageResult { get; set; }

        public CalculationResult(string calculationId, int implementationId, double powerFlowLimit, List<VoltageResult>? voltageResult)
        {
            CalculationId = calculationId;
            ImplementationId = implementationId;
            PowerFlowLimit = powerFlowLimit;
            VoltageResult = voltageResult;
        }
        public CalculationResult(string calculationId, int implementationId, double powerFlowLimit)
        {
            CalculationId = calculationId;
            ImplementationId = implementationId;
            PowerFlowLimit = powerFlowLimit;
        }
    }
}
