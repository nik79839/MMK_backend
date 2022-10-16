using Domain;
using Domain.Result;

namespace Application.Interfaces
{
    public interface ICalculationResultRepository
    {
        Task<List<Calculations>> GetCalculations();
        Task<List<PowerFlowResult>> GetPowerFlowResultById(string? id);
        Task<List<VoltageResult>> GetVoltageResultById(string? id);
        Task AddCalculation(Calculations calculations);
        Task UpdateCalculation(Calculations calculations);
        Task AddPowerFlowResults(List<PowerFlowResult> powerFlowResults);
        Task AddVoltageResults(List<VoltageResult> voltageResults);
        Task DeleteCalculationsById(string? id);
    }
}
