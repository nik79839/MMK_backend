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

    public class VoltageResult : CalculationResultBase
    {
        /// <summary>
        /// Номер узла
        /// </summary>
        public int NodeNumber { get; set; }

        /// <summary>
        /// Номер реализации
        /// </summary>
        public string? NodeName { get; set; }

        /// <summary>
        /// Значение напряжения
        /// </summary>
        public double VoltageValue { get; set; }

        public VoltageResult(string calculationId, int implementationId, int nodeNumber, string nodeName, double voltageValue)
            : base(calculationId,implementationId)
        {
            NodeNumber = nodeNumber;
            NodeName = nodeName;
            VoltageValue = voltageValue;
        }
    }
}
