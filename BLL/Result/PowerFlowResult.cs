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
    public class PowerFlowResult : CalculationResultBase
    {
        /// <summary>
        /// Значение напряжения
        /// </summary>
        public double PowerFlowLimit { get; set; }

        public PowerFlowResult(string calculationId, int implementationId, double powerFlowLimit) 
            : base(calculationId,implementationId )
        {
            PowerFlowLimit = powerFlowLimit;
        }
    }
}
