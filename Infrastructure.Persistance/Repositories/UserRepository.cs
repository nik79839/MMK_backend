using Application.Interfaces;
using AutoMapper;
using Domain;
using Infrastructure.Persistance.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CalculationResultContext _context;
        private readonly IMapper _mapper;

        public UserRepository(CalculationResultContext context)
        {
            _context = context;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserEntity, User>().ReverseMap();
            });
            _mapper = new Mapper(config);
        }

        public async Task<List<User>> GetAllUsers()
        {
            return _mapper.Map<List<User>>(_mapper.Map<List<User>>(_context.Users.ToList()));
        }
    }
}
