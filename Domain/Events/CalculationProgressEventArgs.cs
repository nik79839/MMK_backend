﻿namespace Domain.Events
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
        public Guid CalculationId { get; set; }

        public CalculationProgressEventArgs(Guid calculationId, int percent, int time)
        {
            CalculationId = calculationId;
            Percent = percent;
            Time = time;
        }
    }
}
