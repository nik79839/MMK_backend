namespace Domain
{
    /// <summary>
    /// Параметры утяжеления
    /// </summary>
    public class WorseningSettings
    {
        public Guid CalculationId { get; set; }
        public int NodeNumber { get; set; }
        public WorseningSettings(Guid calculationId, int nodeNumber)
        {
            CalculationId = calculationId;
            NodeNumber = nodeNumber;
        }
    }
}
