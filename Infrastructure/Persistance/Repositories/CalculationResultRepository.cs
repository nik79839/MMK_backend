using Application.Interfaces;
using AutoMapper;
using Domain;
using Domain.InitialResult;
using Infrastructure.Persistance.Entities;
using Infrastructure.Persistance.Entities.Result;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class CalculationResultRepository : ICalculationResultRepository
    {
        private readonly CalculationResultContext _context;
        private readonly IMapper _mapper;

        public CalculationResultRepository(CalculationResultContext context)
        {
            _context = context;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CalculationEntity, Calculations>().ReverseMap();
                cfg.CreateMap<PowerFlowResultEntity, PowerFlowResult>().ReverseMap();
                cfg.CreateMap<VoltageResultEntity, VoltageResult>().ReverseMap();
                cfg.CreateMap<CurrentResultEntity, CurrentResult>().ReverseMap();
                cfg.CreateMap<WorseningSettingsEntity, WorseningSettings>().ReverseMap();
            });
            _mapper = new Mapper(config);

        }

        public async Task AddCalculation(Calculations calculations)
        {
            _context.Calculations.Add(_mapper.Map<CalculationEntity>(calculations));
            await _context.SaveChangesAsync();
        }

        public async Task AddPowerFlowResults(List<PowerFlowResult> powerFlowResults)
        {
            await _context.PowerFlowResults.AddRangeAsync(_mapper.Map<List<PowerFlowResult>, List<PowerFlowResultEntity>>(powerFlowResults));
            await _context.SaveChangesAsync();
        }

        public async Task AddVoltageResults(List<VoltageResult> voltageResults)
        {
            await _context.VoltageResults.AddRangeAsync(_mapper.Map<List<VoltageResultEntity>>(voltageResults));
            await _context.SaveChangesAsync();
        }

        public async Task AddWorseningSettings(List<WorseningSettings> worseningSettings)
        {
            await _context.WorseningSettings.AddRangeAsync(_mapper.Map<List<WorseningSettingsEntity>>(worseningSettings));
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCalculationsById(string? id)
        {
            CalculationEntity calculations1 = (from calculations in _context.Calculations 
                                               where calculations.Id.ToString() == id 
                                               select calculations).FirstOrDefault();
            List<PowerFlowResultEntity> calculationResults = (from calculations in _context.PowerFlowResults 
                                                              where calculations.CalculationId.ToString() == id 
                                                              select calculations).ToList();
            List<VoltageResultEntity> voltageResults = (from calculations in _context.VoltageResults 
                                                        where calculations.CalculationId.ToString() == id 
                                                        select calculations).ToList();
            _context.Calculations.Remove(calculations1);
            _context.PowerFlowResults.RemoveRange(calculationResults);
            _context.VoltageResults.RemoveRange(voltageResults);
            _context.SaveChanges();
        }

        public async Task<List<Calculations>> GetCalculations()
        {
            var calculations = _context.Calculations.OrderByDescending(c => c.CalculationEnd).ToList();
            return _mapper.Map<List<Calculations>>(calculations);
        }

        public async Task<List<PowerFlowResult>> GetPowerFlowResultById(string? id)
        {
            var powerFlowResults = (from calculations in _context.PowerFlowResults 
                                    where calculations.CalculationId.ToString() == id select calculations).ToList();
            return _mapper.Map<List<PowerFlowResult>>(powerFlowResults);
        }

        public async Task<List<VoltageResult>> GetVoltageResultById(string? id)
        {
            var voltageResults = (from calculations in _context.VoltageResults 
                                  where calculations.CalculationId.ToString() == id select calculations).ToList();
            return _mapper.Map<List<VoltageResult>>(voltageResults);
        }

        public async Task<List<WorseningSettings>> GetWorseningSettingsById(string? id)
        {
            var worseningSettings = (from worseningSettings1 in _context.WorseningSettings
                                     where worseningSettings1.CalculationId.ToString() == id
                                     select worseningSettings1).ToList();
            return _mapper.Map<List<WorseningSettings>>(worseningSettings);
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
