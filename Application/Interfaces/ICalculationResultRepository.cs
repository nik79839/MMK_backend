using Domain;
using Domain.InitialResult;

namespace Application.Interfaces
{
    public interface ICalculationResultRepository
    {
        Task<List<Calculations>> GetCalculations();
        Task<CalculationResultInitial> GetResultInitialById(string? id);
        Task AddCalculation(Calculations calculations);
        Task UpdateCalculation(Calculations calculations);
        Task AddPowerFlowResults(List<CalculationResultBase> powerFlowResults);
        Task AddVoltageResults(List<VoltageResult> voltageResults);
        Task AddCurrentResults(List<CurrentResult> currentResults);
        Task AddWorseningSettings(List<WorseningSettings> worseningSettings);
        Task<List<WorseningSettings>> GetWorseningSettingsById(string? id);
        Task DeleteCalculationsById(string? id);
    }
}
