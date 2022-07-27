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
    public class CalculationStatistic
    {

        /// <summary>
        /// Максимум выборки
        /// </summary>
        public double Maximum { get; set; }

        /// <summary>
        /// Высота прямоугольника
        /// </summary>
        public double Minimum { get; set; }

        /// <summary>
        /// Математическое ожидание
        /// </summary>
        public double Mean { get; set; }

        /// <summary>
        /// Среднеквадратическое отклонение
        /// </summary>
        public double StD { get; set; }

        /// <summary>
        ///  Обработанные значения
        /// </summary>
        public List<CalculationResultProcessed> CalculationResultProcessed { get; set; } = new();

        /// <summary>
        ///  Обработанные значения
        /// </summary>
        public List<VoltageResultProcessed> VoltageResultProcessed { get; set; } = new();

        /// <summary>
        /// Обработка результатов расчета
        /// </summary>
        /// <param name="calculationResults"></param>
        public void Processing(List<CalculationResult> calculationResults)
        {
            int intervalCount = Convert.ToInt32(Math.Log10(calculationResults.Count) + Math.Sqrt(calculationResults.Count));
            List<double> values = new();
            foreach (CalculationResult calculationResult in calculationResults)
            {
                values.Add(calculationResult.PowerFlowLimit);
            }
            Maximum = values.Max();
            Minimum = values.Min();
            Mean = Math.Round(values.Average(),2);
            double range = Maximum - Minimum;
            double step = range / intervalCount;
            double sec = Minimum;
            double first;
            for (int i = 0; i < intervalCount; i++)
            {
                int count = 0;
                sec = sec + step;
                first = sec - step;
                for (int j = 0; j < values.Count; j++)
                {
                    if (values[j] >= first && values[j] <= sec)
                    {
                        count++;
                    }
                }
                double height = Convert.ToDouble(count) / Convert.ToDouble(values.Count) / step;
                CalculationResultProcessed.Add(new CalculationResultProcessed() { Interval = Math.Round(first, 2).ToString() + " - " + Math.Round(sec, 2).ToString(), Height = height });
            }
        }

        /// <summary>
        /// Обработка результатов расчета
        /// </summary>
        /// <param name="calculationResults"></param>
        public void Processing(List<VoltageResult> voltageResults)
        {
            var nodeNumbers = voltageResults.Select(x => x.NodeNumber).ToArray().Distinct<int>();
            foreach (var nodeNumber in nodeNumbers)
            {
                List<double> values = new();
                foreach (var voltageResult in voltageResults)
                {
                    if (voltageResult.NodeNumber==nodeNumber)
                    {
                        values.Add(voltageResult.VoltageValue);
                    }
                }
                int intervalCount = Convert.ToInt32(Math.Log10(values.Count) + Math.Sqrt(values.Count));
                double maximum = values.Max();
                double minimum = values.Min();
                double mean = Math.Round(values.Average(), 2);
                double range = maximum - minimum;
                double step = range / intervalCount;
                double sec = minimum;
                double first;
                for (int i = 0; i < intervalCount; i++)
                {
                    int count = 0;
                    sec = sec + step;
                    first = sec - step;
                    for (int j = 0; j < values.Count; j++)
                    {
                        if (values[j] >= first && values[j] <= sec)
                        {
                            count++;
                        }
                    }
                    double height = Convert.ToDouble(count) / Convert.ToDouble(values.Count) / step;
                    VoltageResultProcessed.Add(new VoltageResultProcessed() { Interval = Math.Round(first, 2).ToString() + " - " + Math.Round(sec, 2).ToString(), Height = height, NodeNumber=nodeNumber });
                }
            }           
        }
    }
}
