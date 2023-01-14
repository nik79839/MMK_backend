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

        public async Task AddCalculation(Calculations calculations, int? userId = null)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            var calculation = _mapper.Map<CalculationEntity>(calculations);
            calculation.User = user;
            _context.Calculations.Add(_mapper.Map<CalculationEntity>(calculations));
            await _context.SaveChangesAsync();
        }

        public async Task AddCalculationResults(IEnumerable<CalculationResultBase> calculationResults)
        {
            foreach (var calculationResult in calculationResults)
            {
                switch (calculationResult)
                {
                    case PowerFlowResult powerFlowResult:
                        await _context.PowerFlowResults.AddAsync(_mapper.Map<PowerFlowResult, PowerFlowResultEntity>(powerFlowResult));
                        break;
                    case VoltageResult voltageResult:
                        await _context.VoltageResults.AddAsync(_mapper.Map<VoltageResult, VoltageResultEntity>(voltageResult));
                        break;
                    case CurrentResult currentResult:
                        await _context.CurrentResults.AddAsync(_mapper.Map<CurrentResult, CurrentResultEntity>(currentResult));
                        break;
                }
            }
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
            List<WorseningSettingsEntity> worseningSettings = (from calculations in _context.WorseningSettings
                                                        where calculations.CalculationId.ToString() == id
                                                        select calculations).ToList();
            _context.Calculations.Remove(calculations1);
            _context.WorseningSettings.RemoveRange(worseningSettings);
            _context.PowerFlowResults.RemoveRange(calculationResults);
            _context.VoltageResults.RemoveRange(voltageResults);
            _context.SaveChanges();
        }

        public async Task<List<Calculations>> GetCalculations()
        {
            var calculationsEntity = _context.Calculations.OrderByDescending(c => c.CalculationEnd).ToList();
            var worseningSettingsEntity = _context.WorseningSettings.ToList();
            List<Calculations> calculations = _mapper.Map<List<Calculations>>(calculationsEntity);
            List<WorseningSettings> worseningSettings = _mapper.Map<List<WorseningSettings>>(worseningSettingsEntity);
            foreach (var calculation in calculations)
            {
                calculation.WorseningSettings.AddRange(from worseningSetting in worseningSettings
                                                       where calculation.Id == worseningSetting.CalculationId
                                                       select worseningSetting);
            }
            return calculations;
        }

        public async Task<IEnumerable<CalculationResultBase>> GetResultInitialById(string? id)
        {
            List<CalculationResultBase> calculationResults = new();
            var powerFlowResults = (from calculations in _context.PowerFlowResults
                                    where calculations.CalculationId.ToString() == id
                                    select calculations).ToList();
            var voltageResults = (from calculations in _context.VoltageResults
                                  where calculations.CalculationId.ToString() == id
                                  select calculations).ToList();
            var currentResults = (from calculations in _context.CurrentResults
                                  where calculations.CalculationId.ToString() == id
                                  select calculations).ToList();
            calculationResults.AddRange(_mapper.Map<List<PowerFlowResult>>(powerFlowResults));
            calculationResults.AddRange(_mapper.Map<List<VoltageResult>>(voltageResults));
            calculationResults.AddRange(_mapper.Map<List<CurrentResult>>(currentResults));
            return calculationResults;
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
            string endTime = DateTime.Now.ToString("g");
            calculation1.CalculationEnd = endTime;
            _context.Calculations.Update(calculation1);
            await _context.SaveChangesAsync();
        }
    }
}
