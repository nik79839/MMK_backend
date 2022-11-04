using Domain;
using Domain.Events;

namespace Application.Interfaces
{
    public interface ICalculationService
    {
        public event EventHandler<CalculationProgressEventArgs> CalculationProgress;
        public List<Calculations> GetCalculations();
        public CalculationResultInfo GetCalculationsById(string id);
        public Task StartCalculation(Calculations calculations, CalculationSettings calculationSettings, CancellationToken cancellationToken);
        public Task DeleteCalculationById(string id);
    }
}
