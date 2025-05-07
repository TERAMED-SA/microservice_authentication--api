using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using microservice_authentication__api.src.Application.Services;
using microservice_authentication__api.src.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace microservice_authentication__api.src.Infrastructure.Auth
{
    public class JwtAuthService(UserManager<User> userManager) : ITokenService
    {
        private readonly string _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                         ?? throw new InvalidOperationException("A variável de ambiente 'JWT_SECRET_KEY' não foi definida.");
        private readonly string? _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                      ?? throw new InvalidOperationException("A variável de ambiente 'JWT_ISSUER' não foi definida.");
        private readonly string? _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                        ?? throw new InvalidOperationException("A variável de ambiente 'JWT_AUDIENCE' não foi definida.");
        private readonly int _expireMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRE_MINUTES") ?? "1440");
        private readonly UserManager<User> _userManager = userManager;

        public async Task<string> GenerateToken(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
        {
            new("id", user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            // adiciona os roles como claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public JwtSecurityToken DecryptToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken; // Returns JwtSecurityToken with claims
        }
        public ClaimsPrincipal VerifyToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                ValidateLifetime = true,
                IssuerSigningKey = key,
                ValidateIssuerSigningKey = true
            };

            var handler = new JwtSecurityTokenHandler();
            return handler.ValidateToken(token, tokenValidationParameters, out _);
        }
    }
}