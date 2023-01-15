using Domain;
using Domain.Events;
using Domain.InitialResult;

namespace Application.Interfaces
{
    public interface ICalculationService
    {
        public event EventHandler<CalculationProgressEventArgs> CalculationProgress;
        public List<Calculations> GetCalculations();
        public IEnumerable<CalculationResultBase> GetCalculationResultById(string id);
        public Task StartCalculation(CalculationSettings calculationSettings, CancellationToken cancellationToken, int? userId = null);
        public Task DeleteCalculationById(string id);
    }
}
