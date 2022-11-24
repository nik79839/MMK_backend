using Application.DTOs;
using Application.DTOs.InitialResult;
using Application.DTOs.ProcessedResult;
using AutoMapper;
using Domain;
using Domain.InitialResult;
using Domain.ProcessedResult;

namespace Server
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Calculations, CalculationDto>().ReverseMap();
            CreateMap<CalculationSettingsRequest, CalculationSettings>().ReverseMap();
            CreateMap<WorseningSettingsDto, WorseningSettings>().ReverseMap();
            CreateMap<VoltageResult, VoltageResultDto>().ForMember(m => m.Value,
                    opt => opt.MapFrom(src => src.VoltageValue));
            CreateMap<HistogramData, HistogramDataDto>().ReverseMap();
            CreateMap<StatisticBase, StatisticBaseDto>().ReverseMap();
            CreateMap<VoltageResultProcessed, VoltageResultProcessedDto>().ReverseMap();
            
        }
    }
}