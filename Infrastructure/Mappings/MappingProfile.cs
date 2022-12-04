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
            CreateMap<CalculationResultBaseEntity, CalculationResultBase>().ReverseMap();
            CreateMap<VoltageResultEntity, VoltageResult>().ReverseMap();
            CreateMap<CurrentResultEntity, CurrentResult>().ReverseMap();
        }
    }
}
