using Data;
using Data.Statistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICalculationService
    {
        public List<Calculations> GetCalculations();
        public CalculationStatistic GetCalculationsById(string id);
        public Task StartCalculation(Calculations calculations, CalculationSettings calculationSettings, CancellationToken cancellationToken);
        public Task DeleteCalculationById(string id);
    }
}
