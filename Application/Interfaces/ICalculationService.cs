using Domain;
using Domain.Statistic;

namespace Application.Interfaces
{
    public interface ICalculationService
    {
        public List<Calculations> GetCalculations();
        public CalculationStatistic GetCalculationsById(string id);
        public Task StartCalculation(Calculations calculations, CalculationSettings calculationSettings, CancellationToken cancellationToken);
        public Task DeleteCalculationById(string id);
    }
}
