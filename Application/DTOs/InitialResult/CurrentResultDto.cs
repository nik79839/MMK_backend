namespace Application.DTOs.InitialResult
{
    public class CurrentResultDto
    {
        public int ImplementationId { get; set; }
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
        public double Value { get; set; }
    }
}
