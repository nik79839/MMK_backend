using Domain;
using Domain.InitialResult;

namespace Application.Interfaces
{
    public interface ICalculationResultRepository
    {
        Task<List<Calculations>> GetCalculations();
        Task<Calculations> GetCalculationById(string id);
        Task<IEnumerable<CalculationResultBase>>? GetResultInitialById(string? id);
        Task AddCalculation(Calculations calculations, int? userId = null);
        Task AddCalculationResults(IEnumerable<CalculationResultBase> calculationResults);
        Task UpdateCalculation(Calculations calculations);
        Task AddWorseningSettings(List<WorseningSettings> worseningSettings);
        Task<List<WorseningSettings>> GetWorseningSettingsById(string? id);
        Task DeleteCalculationsById(string? id);
    }
}
