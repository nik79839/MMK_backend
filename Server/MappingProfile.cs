using AutoMapper;
using Data;
using Data.Entities;

namespace Server
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CalculationEntity, Calculations>().ReverseMap();
        }
    }
}

