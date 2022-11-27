namespace Application.DTOs.InitialResult
{
    public class VoltageResultDto
    {
        public int ImplementationId { get; set; }
        public int NodeNumber { get; set; }
        /// <summary>
        /// Номер реализации
        /// </summary>
        public string? NodeName { get; set; }

        /// <summary>
        /// Значение напряжения
        /// </summary>
        public double Value { get; set; }
    }
}
