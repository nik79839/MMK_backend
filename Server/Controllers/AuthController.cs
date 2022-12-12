using Application.DTOs;
using Application.DTOs.Response;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = _authService.GetUsers().Result;
            var response = new GetUsersResponse
            {
                UserAmount = users.Count,
                Users = _mapper.Map<List<User>, List<UserDto>>(users)
            };
            return Ok(response);
        }
    }
}
