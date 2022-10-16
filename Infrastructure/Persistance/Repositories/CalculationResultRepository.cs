using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.Result;
using Infrastructure.Persistance.Entities;
using Infrastructure.Persistance.Entities.Result;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class CalculationResultRepository : ICalculationResultRepository
    {
        private readonly CalculationResultContext _context;
        private readonly IMapper _mapper;

        public CalculationResultRepository(CalculationResultContext context, IMapper mapper)
        {
            _context = context;
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CalculationEntity, Calculations>().ReverseMap();
                cfg.CreateMap<PowerFlowResultEntity, PowerFlowResult>().ReverseMap();
                cfg.CreateMap<VoltageResultEntity, VoltageResult>().ReverseMap();
                cfg.CreateMap<CurrentResultEntity, CurrentResult>().ReverseMap();
            }).CreateMapper();
        }

        public async Task AddCalculation(Calculations calculations)
        {
            _context.Calculations.Add(_mapper.Map<Calculations, CalculationEntity>(calculations));
            await _context.SaveChangesAsync();
        }

        public async Task AddPowerFlowResults(List<PowerFlowResult> powerFlowResults)
        {
            await _context.PowerFlowResults.AddRangeAsync(_mapper.Map<List<PowerFlowResult>, List<PowerFlowResultEntity>>(powerFlowResults));
            await _context.SaveChangesAsync();
        }

        public async Task AddVoltageResults(List<VoltageResult> voltageResults)
        {
            await _context.VoltageResults.AddRangeAsync(_mapper.Map<List<VoltageResult>, List<VoltageResultEntity>>(voltageResults));
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

        public async Task<List<Calculations>> GetCalculations()
        {
            var calculations = _context.Calculations.OrderByDescending(c => c.CalculationEnd).ToList();
            return _mapper.Map<List<CalculationEntity>, List<Calculations>>(calculations);
        }

        public async Task<List<PowerFlowResult>> GetPowerFlowResultById(string? id)
        {
            var powerFlowResults = (from calculations in _context.PowerFlowResults 
                                    where calculations.CalculationId == id select calculations).ToList();
            return _mapper.Map<List<PowerFlowResultEntity>, List<PowerFlowResult>>(powerFlowResults);
        }

        public async Task<List<VoltageResult>> GetVoltageResultById(string? id)
        {
            var voltageResults = (from calculations in _context.VoltageResults 
                                  where calculations.CalculationId == id select calculations).ToList();
            return _mapper.Map<List<VoltageResultEntity>, List<VoltageResult>>(voltageResults);
        }

        public async Task UpdateCalculation(Calculations calculations)
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
