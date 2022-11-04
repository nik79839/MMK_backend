namespace Domain.InitialResult
{
    public class PowerFlowResult : CalculationResultBase
    {
        /// <summary>
        /// Значение напряжения
        /// </summary>
        public double PowerFlowLimit { get; set; }

        public PowerFlowResult(Guid calculationId, int implementationId, double powerFlowLimit)
            : base(calculationId, implementationId)
        {
            PowerFlowLimit = powerFlowLimit;
        }
    }
}
