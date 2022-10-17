using AutoMapper;
using Domain;
using Domain.Result;
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
            CreateMap<VoltageResultEntity, VoltageResult>().ReverseMap();
            CreateMap<CurrentResultEntity, CurrentResult>().ReverseMap();
        }
    }
}
