using Application.DTOs;
using AutoMapper;
using Domain;
using Infrastructure.Persistance.Entities;

namespace Server
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Calculations, CalculationDto>().ReverseMap();
        }
    }
}

