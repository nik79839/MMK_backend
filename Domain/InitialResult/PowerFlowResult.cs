using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.InitialResult
{
    public class PowerFlowResult : CalculationResultBase
    {
        public PowerFlowResult(Guid calculationId, int implementationId, double value) : base(calculationId, implementationId, value)
        {
        }
    }
}
