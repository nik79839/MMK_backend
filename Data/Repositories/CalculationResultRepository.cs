using Data.Repositories.Abstract;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities.Result;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class CalculationResultRepository : ICalculationResultRepository
    {
        private readonly CalculationResultContext _context;

        public CalculationResultRepository(CalculationResultContext context)
        {
            _context = context;
        }

        public async Task AddCalculation(CalculationEntity calculations)
        {
            _context.Calculations.Add(calculations);
            await _context.SaveChangesAsync();
        }

        public async Task AddPowerFlowResults(List<PowerFlowResultEntity> powerFlowResults)
        {
            await _context.PowerFlowResults.AddRangeAsync(powerFlowResults);
            await _context.SaveChangesAsync();
        }

        public async Task AddVoltageResults(List<VoltageResultEntity> voltageResults)
        {
            await _context.VoltageResults.AddRangeAsync(voltageResults);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCalculationsById(string? id)
        {
            CalculationEntity calculations1 = (from calculations in _context.Calculations where calculations.Id == id select calculations).FirstOrDefault();
            List<PowerFlowResultEntity> calculationResults = (from calculations in _context.PowerFlowResults where calculations.CalculationId == id select calculations).ToList();
            List<VoltageResultEntity> voltageResults = (from calculations in _context.VoltageResults where calculations.CalculationId == id select calculations).ToList();
            _context.Calculations.Remove(calculations1);
            _context.PowerFlowResults.RemoveRange(calculationResults);
            _context.VoltageResults.RemoveRange(voltageResults);
            _context.SaveChanges();
        }

        public async Task<List<CalculationEntity>> GetCalculations()
        {
            return _context.Calculations.OrderByDescending(c => c.CalculationEnd).ToList();
        }

        public async Task<List<PowerFlowResultEntity>> GetPowerFlowResultById(string? id)
        {
            return (from calculations in _context.PowerFlowResults where calculations.CalculationId == id select calculations).ToList();
        }

        public async Task<List<VoltageResultEntity>> GetVoltageResultById(string? id)
        {
            return (from calculations in _context.VoltageResults where calculations.CalculationId == id select calculations).ToList();
        }

        public async Task UpdateCalculation(CalculationEntity calculations)
        {
            _context.ChangeTracker.Clear();
            CalculationEntity calculation1 = _context.Calculations.AsNoTracking().FirstOrDefault(u => u.Id == calculations.Id);
            DateTime endTime = DateTime.UtcNow;
            calculation1.CalculationEnd = endTime;
            _context.Calculations.Update(calculation1);
            await _context.SaveChangesAsync();
        }
    }
}
