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

        public VoltageResult(Guid calculationId, int implementationId, int nodeNumber, string nodeName, double value)
            : base(calculationId, implementationId, value)
        {
            NodeNumber = nodeNumber;
            NodeName = nodeName;
        }
    }
}
