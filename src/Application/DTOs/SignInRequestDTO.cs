using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_authentication__api.src.Application.DTOs
{
    public class SignInRequestDTO
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}