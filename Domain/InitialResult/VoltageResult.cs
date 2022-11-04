namespace Domain.InitialResult
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

        public VoltageResult(Guid calculationId, int implementationId, int nodeNumber, string nodeName, double voltageValue)
            : base(calculationId, implementationId)
        {
            NodeNumber = nodeNumber;
            NodeName = nodeName;
            VoltageValue = voltageValue;
        }
    }
}
