using Model.Statistic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Result
{
    public class CurrentResult
    {
        /// <summary>
        /// Уникальный идентификатор расчета
        /// </summary>
        [Key, Column(Order = 0)]
        public string CalculationId { get; set; }

        /// <summary>
        /// Номер реализации
        /// </summary>
        [Key, Column(Order = 1)]
        public int ImplementationId { get; set; }

        /// <summary>
        /// Номер реализации
        /// </summary>
        [Key, Column(Order = 2)]
        public int StartNode { get; set; }
        /// <summary>
        /// Номер реализации
        /// </summary>
        [Key, Column(Order = 3)]
        public int EndNode { get; set; }

        /// <summary>
        /// Значение напряжения
        /// </summary>
        public double CurrentValue { get; set; }

        /// <summary>
        /// Ссылка на расчет
        /// </summary>
        public Calculations? Calculations { get; set; }

        public CurrentResult(string calculationId, int implementationId, int startNode, int endNode, double currentValue)
        {
            CalculationId = calculationId;
            ImplementationId = implementationId;
            StartNode = startNode;
            EndNode = endNode;
            CurrentValue = currentValue;
        }
    }
}
