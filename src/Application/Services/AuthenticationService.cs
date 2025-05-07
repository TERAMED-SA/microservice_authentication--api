using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using microservice_authentication__api.src.Application.Common.Response;
using microservice_authentication__api.src.Application.DTOs;
using microservice_authentication__api.src.Domain.Entities;
using microservice_authentication__api.src.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;

namespace microservice_authentication__api.src.Application.Services
{
    public class AuthenticationService(ITokenService tokenService, UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IHttpContextAccessor httpContextAccessor) : IAuthenticationService
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IMapper _mapper = mapper;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<Result<MeDTO>> Me(string token)
        {
            var validateToken = _tokenService.DecryptToken(token);
            if (validateToken == null)
                return Result<MeDTO>.Failure(401, "Erro", "Token inválido.");
            var userId = validateToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Result<MeDTO>.Failure(400, "Erro", "ID do usuário não encontrado");
            if (!Guid.TryParse(userId, out Guid userGuid))
                return Result<MeDTO>.Failure(400, "Erro", "ID do usuário inválido");
            var findUser = await _userManager.FindByIdAsync(userId);
            var meDto = new MeDTO
            {
                Id = findUser!.Id,
                Email = findUser.Email!,
                UserName = findUser.UserName!,
                FirstName = findUser.FirstName,
                LastName = findUser.LastName,
                CreatedAt = findUser.CreatedAt,
                UpdatedAt = findUser.UpdatedAt,
                Roles = await _userManager.GetRolesAsync(findUser),
            };
            return Result<MeDTO>.Success(meDto, 200, "Sucesso");
        }
        public async Task<Result<string>> SignInAsync(SignInRequestDTO request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return Result<string>.Failure(401, "Erro", "Credenciais inválidas.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return Result<string>.Failure(401, "Erro", "Credenciais inválidas.");

            return Result<string>.Success($"{await _tokenService.GenerateToken(user)}", 200, "Sucesso");
        }


        public async Task<Result<string>> RegisterPatientAsync(SignUpRequestDTO dto)
        {
            var currentUserRole = GetCurrentUserRole();
            if (currentUserRole != RolesNames.Admin && currentUserRole != RolesNames.Manager)
                return Result<string>.Failure(403, "Erro", "Apenas Admin ou Manager podem criar um Paciente.");
            return await RegisterUserWithRole(dto, RolesNames.Patient);
        }

        public async Task<Result<string>> RegisterProfessionalAsync(SignUpRequestDTO dto)
        {
            var currentUserRole = GetCurrentUserRole();
            if (currentUserRole != RolesNames.Admin && currentUserRole != RolesNames.Manager)
                return Result<string>.Failure(403, "Erro", "Apenas Admin ou Manager podem criar um Professional.");
            return await RegisterUserWithProfessionalRoles(dto);
        }

        public async Task<Result<string>> RegisterAdminAsync(SignUpRequestDTO dto)
        {
            var role = GetCurrentUserRole();
            if (role != RolesNames.Admin)
                return Result<string>.Failure(403, "Erro", "Apenas Admin pode criar outro Admin.");

            return await RegisterUserWithRole(dto, RolesNames.Admin);
        }
        public async Task<Result<string>> RegisterManagerAsync(SignUpRequestDTO dto)
        {
            var currentUserRole = GetCurrentUserRole();
            if (currentUserRole != RolesNames.Admin)
                return Result<string>.Failure(403, "Erro", "Apenas Admin pode criar um Manager.");

            return await RegisterUserWithRole(dto, RolesNames.Manager);
        }

        private async Task<Result<string>> RegisterUserWithRole(SignUpRequestDTO dto, string role)
        {
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.Email,
                Email = dto.Email
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return Result<string>.Failure(500, "Erro", "Erro ao criar o usuário.");
            await _userManager.AddToRoleAsync(user, role);
            return Result<string>.Success($"{await _tokenService.GenerateToken(user)}", 201, "Sucesso");
        }

        private async Task<Result<string>> RegisterUserWithProfessionalRoles(SignUpRequestDTO dto)
        {
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.Email,
                Email = dto.Email
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return Result<string>.Failure(500, "Erro", "Erro ao criar o usuário.");

            await _userManager.AddToRoleAsync(user, RolesNames.Professional);
            await _userManager.AddToRoleAsync(user, RolesNames.Scheduling);
            await _userManager.AddToRoleAsync(user, RolesNames.Appointment);
            return Result<string>.Success($"{await _tokenService.GenerateToken(user)}", 201, "Sucesso");
        }

        private string? GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }

    }
}