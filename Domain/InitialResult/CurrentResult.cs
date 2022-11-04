namespace Domain.InitialResult
{
    public class CurrentResult : CalculationResultBase
    {
        /// <summary>
        /// Номер реализации
        /// </summary>
        public int StartNode { get; set; }
        /// <summary>
        /// Номер реализации
        /// </summary>
        public int EndNode { get; set; }

        /// <summary>
        /// Значение напряжения
        /// </summary>
        public double CurrentValue { get; set; }

        public CurrentResult(Guid calculationId, int implementationId, int startNode, int endNode, double currentValue)
            : base(calculationId, implementationId)
        {
            StartNode = startNode;
            EndNode = endNode;
            CurrentValue = currentValue;
        }
    }
}
