using Model.Statistic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Result
{
    public class VoltageResult
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
        /// Номер узла
        /// </summary>
        [Key, Column(Order = 2)]
        public int NodeNumber { get; set; }

        /// <summary>
        /// Номер реализации
        /// </summary>
        [NotMapped]
        public string? NodeName { get; set; }

        /// <summary>
        /// Значение напряжения
        /// </summary>
        public double VoltageValue { get; set; }

        /// <summary>
        /// Ссылка на расчет
        /// </summary>
        public Calculations? Calculations { get; set; }

        public VoltageResult(string calculationId, int implementationId, int nodeNumber, string? nodeName, double voltageValue)
        {
            CalculationId = calculationId;
            ImplementationId = implementationId;
            NodeNumber = nodeNumber;
            NodeName = nodeName;
            VoltageValue = voltageValue;
        }
    }
}
