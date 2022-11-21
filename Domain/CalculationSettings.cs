using Domain.Rastrwin3.RastrModel;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    /// <summary>
    /// Параметры расчета, приходящие от клиента
    /// </summary>
    public class CalculationSettings
    {
        /// <summary>
        /// Название расчета
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание расчета
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Узлы варьируемых нагрузок
        /// </summary>
        public List<Node>? LoadNodes { get; set; }

        /// <summary>
        /// Узлы утяжеления нагрузок
        /// </summary>
        public List<WorseningSettings> WorseningSettings { get; set; }

        /// <summary>
        /// Число реализаций
        /// </summary>
        public int CountOfImplementations { get; set; }

        /// <summary>
        /// Путь к файлу формата .rg2
        /// </summary>
        public string? PathToRegim { get; set; }

        /// <summary>
        /// Путь к файлу формата sch
        /// </summary>
        public string? PathToSech { get; set; }

        /// <summary>
        /// Номер сечения
        /// </summary>
        public int SechNumber { get; set; }

        /// <summary>
        /// Диапазон начального состояния нагрузок
        /// </summary>
        public int PercentLoad { get; set; }

        /// <summary>
        /// Процент приращения при утяжелениии
        /// </summary>
        public int PercentForWorsening { get; set; }

        /// <summary>
        /// Все ли узлы имеют случайный начальный характер
        /// </summary>
        public bool? IsAllNodesInitial { get; set; } = true;
    }
}
