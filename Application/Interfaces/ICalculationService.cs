using Domain;
using Domain.Events;
using Domain.InitialResult;

namespace Application.Interfaces
{
    public interface ICalculationService
    {
        public event EventHandler<CalculationProgressEventArgs> CalculationProgress;
        public List<Calculations> GetCalculations();
        public CalculationResultInitial GetCalculationsById(string id);
        public Task StartCalculation(CalculationSettings calculationSettings, CancellationToken cancellationToken);
        public Task DeleteCalculationById(string id);
    }
}
