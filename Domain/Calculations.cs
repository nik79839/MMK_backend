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
        public string CalculationStart { get; set; } = DateTime.Now.ToString("g");

        /// <summary>
        /// Время Конца расчета
        /// </summary>
        public string? CalculationEnd { get; set; } = null;

        /// <summary>
        /// Название сечения
        /// </summary>
        public string? SechName { get; set; }
        public string? PathToRegim { get; set; }
        public int PercentLoad { get; set; }
        public int PercentForWorsening { get; set; }

        /// <summary>
        /// Процент прогресса расчета
        /// </summary>
        public int? Progress { get; set; } = null;
        public List<WorseningSettings> WorseningSettings { get; set; } = new();
    }
}
