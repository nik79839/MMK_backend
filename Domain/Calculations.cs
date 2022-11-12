namespace Domain
{
    public class Calculations
    {

        /// <summary>
        /// Уникальный идентификатор расчета
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Название расчета
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание расчета
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Время начала расчета
        /// </summary>
        public DateTime CalculationStart { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Время Конца расчета
        /// </summary>
        public DateTime? CalculationEnd { get; set; } = null;

        /// <summary>
        /// Название сечения
        /// </summary>
        public string? SechName { get; set; }
        public string? PathToRegim { get; set; }
        public int? PercentLoad { get; set; }
        public int? PercentForWorsening { get; set; }

        /// <summary>
        /// Процент прогресса расчета
        /// </summary>
        public int? Progress { get; set; } = null;
        public List<int> NodesForWorsening { get; set; } = new();
    }
}
