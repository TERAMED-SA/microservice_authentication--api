using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using microservice_authentication__api.src.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace microservice_authentication__api.src.Application.Services
{
    public interface ITokenService
    {
        Task<string> GenerateToken(User user);
        JwtSecurityToken DecryptToken(string token);
        ClaimsPrincipal VerifyToken(string token);
    }
}