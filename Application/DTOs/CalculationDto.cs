namespace Application.DTOs
{
    public class CalculationDto
    {
        /// <summary>
        /// Уникальный идентификатор расчета
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Название расчета
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Время Конца расчета
        /// </summary>
        public DateTime? CalculationEnd { get; set; }

        /// <summary>
        /// Название сечения
        /// </summary>
        public string? SechName { get; set; }

        /// <summary>
        /// Процент прогресса расчета
        /// </summary>
        public int? Progress { get; set; } = null;
        public string? PathToRegim { get; set; }
        public int? PercentLoad { get; set; }
        public int? PercentForWorsening { get; set; }
        public string? Description { get; set; }
        public List<int> NodesForWorsening { get; set; }
    }
}
