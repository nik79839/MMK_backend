using Domain.InitialResult;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Calculations
    {
        
        /// <summary>
        /// Уникальный идентификатор расчета
        /// </summary>
        public string Id { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Недопустимая длина имени расчета")]
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
        public DateTime? CalculationEnd { get; set; }

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
    }
}
