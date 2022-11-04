namespace Application.DTOs.InitialResult
{
    public class VoltageResultDto
    {
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
