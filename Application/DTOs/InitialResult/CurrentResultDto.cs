namespace Application.DTOs.InitialResult
{
    public class CurrentResultDto
    {
        public int ImplementationId { get; set; }
        /// <summary>
        /// Номер реализации
        /// </summary>
        public string BrunchName { get; set; }

        /// <summary>
        /// Значение напряжения
        /// </summary>
        public double Value { get; set; }
    }
}
