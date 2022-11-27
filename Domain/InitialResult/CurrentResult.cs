namespace Domain.InitialResult
{
    public class CurrentResult : CalculationResultBase
    {
        /// <summary>
        /// Номер реализации
        /// </summary>
        public string BrunchName { get; set; }

        /// <summary>
        /// Значение напряжения
        /// </summary>
        public double CurrentValue { get; set; }

        public CurrentResult(Guid calculationId, int implementationId, string brunchName, double currentValue)
            : base(calculationId, implementationId)
        {
            BrunchName = brunchName;
            CurrentValue = currentValue;
        }
    }
}
