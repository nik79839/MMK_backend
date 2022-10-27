using AutoMapper;
using Domain;
using Domain.InitialResult;
using Infrastructure.Persistance.Entities;
using Infrastructure.Persistance.Entities.Result;

namespace Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CalculationEntity, Calculations>().ReverseMap();
            CreateMap<PowerFlowResultEntity, PowerFlowResult>().ReverseMap();
            CreateMap<List<PowerFlowResultEntity>,List<PowerFlowResult>>().ReverseMap();
            CreateMap<VoltageResultEntity, VoltageResult>().ReverseMap();
            CreateMap<List<VoltageResultEntity>, List<VoltageResult>>().ReverseMap();
            CreateMap<CurrentResultEntity, CurrentResult>().ReverseMap();
            CreateMap<List<CurrentResultEntity>, List<CurrentResult>>().ReverseMap();
        }
    }
}
