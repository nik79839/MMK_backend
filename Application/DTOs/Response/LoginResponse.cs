using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response
{
    public class LoginResponse
    {
        public string Name { get; set; }
        public string Token { get; set; }

        public LoginResponse(string name, string token)
        {
            Name = name;
            Token = token;
        }
    }
}
