using Model;
using Model.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Abstract
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
