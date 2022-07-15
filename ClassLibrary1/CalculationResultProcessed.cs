using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    /// <summary>
    /// Результаты расчета для хранения в БД
    /// </summary>
    public class CalculationResultProcessed
    {

        /// <summary>
        /// Диапазон
        /// </summary>
        public string Interval { get; set; }

        /// <summary>
        /// Высота прямоугольника
        /// </summary>
        public double Height { get; set; }

        public static List<CalculationResultProcessed> Processing(List<CalculationResult> calculationResults)
        {
            List<CalculationResultProcessed> calculationResultProcessed = new List<CalculationResultProcessed>();
            int intervalCount = Convert.ToInt32(Math.Log10(calculationResults.Count) + Math.Sqrt(calculationResults.Count));
            List<double> values = new();
            foreach (CalculationResult calculationResult in calculationResults)
            {
                values.Add(calculationResult.PowerFlowLimit);
            }
            double max = values.Max();
            double min = values.Min();
            double average = values.Average();
            double range = max - min;
            double diap = range / intervalCount;
            double sec = min;
            double first;
            for (int i = 0; i < intervalCount; i++)
            {
                int count = 0;
                sec = sec + diap;
                first = sec - diap;
                for (int j = 0; j < values.Count; j++)
                {
                    if (values[j] >= first && values[j] <= sec)
                    {
                        count++;
                    }
                }
                double height = Convert.ToDouble(count) / Convert.ToDouble(values.Count)/diap;
                calculationResultProcessed.Add(new CalculationResultProcessed() { Interval = Math.Round(first,2).ToString() + " - " + Math.Round(sec,2).ToString(), Height = height });
            }
            return calculationResultProcessed;
        }
    }
}
