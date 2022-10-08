using Data.Repositories.Abstract;
using Model;
using Model.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class CalculationResultRepository : ICalculationResultRepository
    {
        private readonly CalculationResultContext _context;

        public CalculationResultRepository(CalculationResultContext context)
        {
            _context = context;
        }

        public async Task AddCalculation(Calculations calculations)
        {
            await _context.Calculations.AddAsync(calculations);
            await _context.SaveChangesAsync();
        }

        public async Task AddPowerFlowResults(List<PowerFlowResult> powerFlowResults)
        {
            await _context.PowerFlowResults.AddRangeAsync(powerFlowResults);
        }

        public async Task AddVoltageResults(List<VoltageResult> voltageResults)
        {
            await _context.VoltageResults.AddRangeAsync(voltageResults);
        }

        public async Task DeleteCalculationsById(string? id)
        {
            Calculations calculations1 = (from calculations in _context.Calculations where calculations.CalculationId == id select calculations).FirstOrDefault();
            List<PowerFlowResult> calculationResults = (from calculations in _context.PowerFlowResults where calculations.CalculationId == id select calculations).ToList();
            List<VoltageResult> voltageResults = (from calculations in _context.VoltageResults where calculations.CalculationId == id select calculations).ToList();
            _context.Calculations.Remove(calculations1);
            _context.PowerFlowResults.RemoveRange(calculationResults);
            _context.VoltageResults.RemoveRange(voltageResults);
            _context.SaveChanges();
        }

        public async Task<List<Calculations>> GetCalculations()
        {
            return _context.Calculations.OrderByDescending(c => c.CalculationEnd).ToList();
        }

        public Task<Calculations> GetCalculationsById(string? id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<PowerFlowResult>> GetPowerFlowResultById(string? id)
        {
            return (from calculations in _context.PowerFlowResults where calculations.CalculationId == id select calculations).ToList();
        }

        public async Task<List<VoltageResult>> GetVoltageResultById(string? id)
        {
            return (from calculations in _context.VoltageResults where calculations.CalculationId == id select calculations).ToList();
        }

        public async Task UpdateCalculation(Calculations calculations)
        {
            _context.Calculations.Update(calculations);
            _context.SaveChanges();
        }
    }
}
