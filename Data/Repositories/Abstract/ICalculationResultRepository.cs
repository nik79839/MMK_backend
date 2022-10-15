using Data;
using Data.Entities;
using Data.Entities.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Abstract
{
    public interface ICalculationResultRepository
    {
        Task<List<CalculationEntity>> GetCalculations();
        Task<List<PowerFlowResultEntity>> GetPowerFlowResultById(string? id);
        Task<List<VoltageResultEntity>> GetVoltageResultById(string? id);
        Task AddCalculation(CalculationEntity calculations);
        Task UpdateCalculation(CalculationEntity calculations);
        Task AddPowerFlowResults(List<PowerFlowResultEntity> powerFlowResults);
        Task AddVoltageResults(List<VoltageResultEntity> voltageResults);
        Task DeleteCalculationsById(string? id);
    }
}
