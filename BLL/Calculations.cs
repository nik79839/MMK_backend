using Data.Events;
using Data.Result;

namespace Data
{
    public class Calculations
    {
        /// <summary>
        /// Событие описания прогресса расчета
        /// </summary>
        public event EventHandler<CalculationProgressEventArgs> CalculationProgress;

        /// <summary>
        /// Уникальный идентификатор расчета
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Название расчета
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Время начала расчета
        /// </summary>
        public DateTime CalculationStart { get; set; }

        /// <summary>
        /// Время Конца расчета
        /// </summary>
        public DateTime? CalculationEnd { get; set; }

        public string? SechName { get; set; }
        public string? PathToRegim { get; set; }
        public int? PercentLoad { get; set; }
        public int? PercentForWorsening { get; set; }

        /// <summary>
        /// Ссылка на результаты расчетов
        /// </summary>
        public List<PowerFlowResult> PowerFlowResults { get; set; } = new();

        /// <summary>
        /// Ссылка на результаты расчетов напряжений
        /// </summary>
        public List<VoltageResult> VoltageResults { get; set; } = new();

        /// <summary>
        /// Ссылка на результаты расчетов токов
        /// </summary>
        public List<CurrentResult> CurrentResults { get; set; } = new();

        /// <summary>
        /// Процент прогресса расчета
        /// </summary>
        public int? Progress { get; set; } = null;
    }
}
