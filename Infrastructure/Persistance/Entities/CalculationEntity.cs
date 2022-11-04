using Infrastructure.Persistance.Entities.Result;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistance.Entities
{
    [Table("Calculation")]
    public class CalculationEntity
    {
        /// <summary>
        /// Уникальный идентификатор расчета
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Название расчета
        /// </summary>
        public string Name { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// Время начала расчета
        /// </summary>
        public DateTime CalculationStart { get; set; }

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
        /// Внешний ключ на Id пользователя
        /// </summary>
        public UserEntity? User { get; set; }

        /// <summary>
        /// Внешний ключ на результаты расчетов
        /// </summary>
        public List<PowerFlowResultEntity> PowerFlowResults { get; set; } = new();

        /// <summary>
        /// Внешний ключ на результаты расчетов напряжений
        /// </summary>
        public List<VoltageResultEntity> VoltageResults { get; set; } = new();

        /// <summary>
        /// Внешний ключ на результаты расчетов токов
        /// </summary>
        public List<CurrentResultEntity> CurrentResults { get; set; } = new();
        public List<WorseningSettingsEntity> WorseningSettings { get; set; } = new();
    }
}
