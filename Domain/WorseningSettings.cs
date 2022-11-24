namespace Domain
{
    /// <summary>
    /// Параметры утяжеления
    /// </summary>
    public class WorseningSettings
    {
        public Guid? CalculationId { get; set; }
        public int NodeNumber { get; set; }
        public int? MaxValue { get; set; }
        public WorseningSettings(Guid? calculationId, int nodeNumber, int? maxValue)
        {
            CalculationId = calculationId;
            NodeNumber = nodeNumber;
            MaxValue = maxValue;
        }

        public WorseningSettings()
        {
            CalculationId = Guid.Empty;
        }
    }
}
