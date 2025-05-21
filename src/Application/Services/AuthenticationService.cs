using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using microservice_authentication__api.src.Application.Common.Response;
using microservice_authentication__api.src.Application.DTOs;
using microservice_authentication__api.src.Application.Interfaces;
using microservice_authentication__api.src.Domain.Entities;
using microservice_authentication__api.src.Infrastructure.Auth;
using microservice_authentication__api.src.Infrastructure.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace microservice_authentication__api.src.Application.Services
{
    public class AuthenticationService(ITokenService tokenService, UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, IHttpContextAccessor httpContextAccessor, IExternalApi externalApi, INotificationService notificationService) : IAuthenticationService
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IMapper _mapper = mapper;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IExternalApi _externalApi = externalApi;
        private readonly INotificationService _notificationService = notificationService;

        #region Login
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
                ExternalReferenceId = findUser.ExternalReferenceId,
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

            if (user.TwoFactorEnabled)
            {
                // Gera token de verificação via SMS
                var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);

                // Envia o código via SMS
                await SendNotication(user.PhoneNumber!, $"Seu código de verificação: {code}");

                // Retorna mensagem solicitando o código
                return Result<string>.Success(null, 206, "Código 2FA enviado via SMS. Por favor, confirme.");
            }

            // Autenticação normal sem 2FA
            var token = await _tokenService.GenerateToken(user);
            return Result<string>.Success(token, 200, "Sucesso");
        }
        #endregion

        #region 2f Métodos
        public async Task<Result<string>> VerifyTwoFactorCodeAsync(string username, string code)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Result<string>.Failure(404, "Erro", "Usuário não encontrado.");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider, code);

            if (!isValid)
                return Result<string>.Failure(401, "Erro", "Código inválido.");

            var token = await _tokenService.GenerateToken(user);
            return Result<string>.Success(token, 200, "Login com 2FA realizado com sucesso");
        }

        public async Task<Result<string>> EnableTwoFactorAsync(string username, string phoneNumber)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Result<string>.Failure(404, "Erro", "Usuário não encontrado.");

            // Define o número de telefone, se ainda não estiver salvo
            if (user.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, phoneNumber);
                if (!setPhoneResult.Succeeded)
                    return Result<string>.Failure(400, "Erro", "Erro ao definir o número de telefone.");
            }

            // Gera e envia código de confirmação por SMS
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

            await SendNotication(phoneNumber, $"Seu código de ativação de 2FA: {code}");
            // Informa ao cliente que o código foi enviado
            return Result<string>.Success(null, 206, "Código de verificação enviado. Por favor, confirme.");
        }

        public async Task<Result<string>> ConfirmTwoFactorPhoneAsync(string username, string code)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Result<string>.Failure(404, "Erro", "Usuário não encontrado.");

            var isValid = await _userManager.VerifyChangePhoneNumberTokenAsync(user, code, user.PhoneNumber!);

            if (!isValid)
                return Result<string>.Failure(400, "Erro", "Código inválido ou expirado.");

            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);
            // Ativa o 2FA
            var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
            if (!result.Succeeded)
                return Result<string>.Failure(500, "Erro", "Erro ao ativar o 2FA.");

            return Result<string>.Success(null, 200, "2FA ativado com sucesso!");
        }
        #endregion

        #region Metodos de registro
        public async Task<Result<string>> RegisterPatientAsync(SignUpRequestDTO dto)
        {
            return await RegisterUserWithRole(dto, RolesNames.Patient);
        }

        public async Task<Result<string>> RegisterProfessionalAsync(SignUpRequestDTO dto)
        {
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
                Email = dto.Email,
                ExternalReferenceId = dto.ExternalReferenceId,
            };
            var generatedPassword = Password.GenerateSecurePassword();
            var result = await _userManager.CreateAsync(user, generatedPassword);
            if (!result.Succeeded)
                return Result<string>.Failure(500, "Erro", "Erro ao criar o usuário.");
            await _userManager.AddToRoleAsync(user, role);
            return Result<string>.Success($"{await _tokenService.GenerateToken(user)}", 201, "Sucesso");
        }

        private async Task<Result<string>> RegisterUserWithProfessionalRoles(SignUpRequestDTO dto)
        {
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.UserName);

            if (existingUser != null)
            {
                return Result<string>.Failure(400, "Erro", "O número de telefone já está registrado.");
            }
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.UserName,
                Email = dto.Email,
                ExternalReferenceId = dto.ExternalReferenceId,
                PhoneNumber = dto.UserName,
            };
            var generatedPassword = Password.GenerateSecurePassword();
            var result = await _userManager.CreateAsync(user, generatedPassword);
            if (!result.Succeeded)
                return Result<string>.Failure(500, "Erro", $"Erro ao criar o usuário. {result}");

            await _userManager.AddToRoleAsync(user, RolesNames.Professional);
            await _userManager.AddToRoleAsync(user, RolesNames.Scheduling);
            await _userManager.AddToRoleAsync(user, RolesNames.Appointment);
            await SendNotication(user.UserName, $"Sua senha é {generatedPassword}");
            return Result<string>.Success($"{await _tokenService.GenerateToken(user)}", 201, "Sucesso");
        }
        #endregion

        #region Recuperar/Update senha
        public async Task<Result<string>> ChangePasswordAsync(string username, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Result<string>.Failure(404, "Erro", "Usuário não encontrado.");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return Result<string>.Failure(400, "Erro", "Senha atual incorreta ou nova senha inválida.");

            return Result<string>.Success(null, 200, "Senha alterada com sucesso.");
        }
        public async Task<Result<string>> RequestPasswordRecoveryAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Result<string>.Failure(404, "Erro", "Usuário não encontrado.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var code = new Random().Next(100000, 999999).ToString();
            PasswordRecoveryMemory.Store(user.UserName!, code, token);

            await SendNotication(user.PhoneNumber!, $"Código de recuperação: {code}");

            return Result<string>.Success(null, 200, "Código de recuperação enviado.");
        }

        public async Task<Result<string>> ResetPasswordAsync(string username, string code, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Result<string>.Failure(404, "Erro", "Usuário não encontrado.");

            var (token, valid) = PasswordRecoveryMemory.Validate(username, code);
            if (!valid)
                return Result<string>.Failure(400, "Erro", "Código inválido ou expirado.");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
                return Result<string>.Failure(400, "Erro", "Erro ao atualizar a senha.");

            return Result<string>.Success(null, 200, "Senha atualizada com sucesso!");
        }

        #endregion

        #region Métodos auxiliares
        private string? GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
        private async Task<Result<string>> SendNotication(string userName, string code)
        {
            var sendNotification = await _notificationService.SendSmsAsync(userName, code);
            if (sendNotification.Error)
            {
                return Result<string>.Failure(500, "Erro", $"{sendNotification.Message}");
            }
            return Result<string>.Success(null, 200, "Sucesso", "Código de validação enviado com sucesso.");
        }
        #endregion


        // public async Task<Result<string>> LogoutAsync()
        // {
        //     return await Task.FromResult(Result<string>.Success(null, 200, "Sucesso"));
        // }
    }
}