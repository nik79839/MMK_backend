using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CalculationProgressEventArgs : EventArgs
    {
       /// <summary>
       /// Прогресс в процентах
       /// </summary>
        public int Percent { get; set; }

        /// <summary>
        /// Оставшееся время в минутах
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// Уникальный идентификатор расчета
        /// </summary>
        public string CalculationId { get; set; }

        public CalculationProgressEventArgs(string calculationId,int percent,int time)
        {
            CalculationId = calculationId;
            Percent = percent;
            Time = time;
        }
    }
}
