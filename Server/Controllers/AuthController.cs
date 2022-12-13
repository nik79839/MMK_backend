using Application.DTOs;
using Application.DTOs.Requests;
using Application.DTOs.Response;
using Application.Interfaces;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        [HttpPost]
        [Route("auth")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            try
            {
                var user = _authService.Login(login.Login, login.Password).Result;
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, $"{user.SurName} {user.Name} {user.LastName}"),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())

                    };
                var jwt = new JwtSecurityToken(issuer: "MyAuthServer", audience: "MyAuthClient", claims: claims, expires: DateTime.UtcNow.Add(TimeSpan.FromDays(3650)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysupersecret_secretkey!123")), SecurityAlgorithms.HmacSha256));
                var token = new JwtSecurityTokenHandler().WriteToken(jwt);
                return Ok(new LoginResponse($"{user.SurName} {user.Name} {user.LastName}",token));
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }        
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser(CreateUserRequest user)
        {
            await _authService.CreateUser(_mapper.Map<CreateUserRequest, User>(user));
            return Ok();

        }

        [Route("whoAmI")]
        [HttpGet]
        public async Task<IActionResult> WhoAmI()
        {
            string name = HttpContext.User.Identity.Name?.ToString();
            return Ok(name);
        }
    }
}
