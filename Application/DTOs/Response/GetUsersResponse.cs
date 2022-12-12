using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response
{
    public class GetUsersResponse
    {
        public int UserAmount { get; set; }
        public List<UserDto> Users { get; set; } = new();
    }
}
