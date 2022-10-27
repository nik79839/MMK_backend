﻿using Domain;
using Domain.InitialResult;

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
        Task AddWorseningSettings(List<WorseningSettings> worseningSettings);
        Task<List<WorseningSettings>> GetWorseningSettingsById(string? id);
        Task DeleteCalculationsById(string? id);
    }
}
