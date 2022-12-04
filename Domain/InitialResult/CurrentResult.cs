namespace Domain.InitialResult
{
    public class CurrentResult : CalculationResultBase
    {
        /// <summary>
        /// Номер реализации
        /// </summary>
        public string BrunchName { get; set; }

        public CurrentResult(Guid calculationId, int implementationId, string brunchName, double value)
            : base(calculationId, implementationId, value)
        {
            BrunchName = brunchName;
        }
    }
}
