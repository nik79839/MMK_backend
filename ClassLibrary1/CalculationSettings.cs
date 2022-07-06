using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    /// <summary>
    /// Параметры расчета, приходящие от клиента
    /// </summary>
    public class CalculationSettings
    {
        /// <summary>
        /// Узлы варьируемых нагрузок
        /// </summary>
        public List<int> NagrNodes { get; set; }

        /// <summary>
        /// Узлы утяжеления нагрузок
        /// </summary>
        public List<int> NodesForWorsening { get; set; }

        /// <summary>
        /// Число реализаций
        /// </summary>
        public int CountOfImplementations { get; set; }

        /// <summary>
        /// Путь к файлу формата .rg2
        /// </summary>
        public string PathToRegim { get; set; }

        /// <summary>
        /// Путь к файлу формата sch
        /// </summary>
        public string PathToSech { get; set; }

        /// <summary>
        /// Диапазон начального состояния нагрузок
        /// </summary>
        public int PercentLoad { get; set; }

        /// <summary>
        /// Процент приращения при утяжелениии
        /// </summary>
        public int PercentForWorsening { get; set; }
    }
}
