using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using microservice_authentication__api.src.Application.Common.Response;
using microservice_authentication__api.src.Application.DTOs;
using microservice_authentication__api.src.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace microservice_authentication__api.src.Application.Services
{
    public interface IAuthenticationService
    {
        Task<Result<string>> SignInAsync(SignInRequestDTO request);
        Task<Result<MeDTO>> Me(string token);
        Task<Result<string>> RegisterAdminAsync(SignUpRequestDTO dto);
        Task<Result<string>> RegisterManagerAsync(SignUpRequestDTO dto);
        Task<Result<string>> RegisterPatientAsync(SignUpRequestDTO dto);
        Task<Result<string>> RegisterProfessionalAsync(SignUpRequestDTO dto);

        Task<Result<string>> VerifyTwoFactorCodeAsync(string username, string code);
        Task<Result<string>> EnableTwoFactorAsync(string username, string phoneNumber);
        Task<Result<string>> ConfirmTwoFactorPhoneAsync(string username, string code);

        Task<Result<string>> ChangePasswordAsync(string username, string currentPassword, string newPassword);
        Task<Result<string>> RequestPasswordRecoveryAsync(string username);
        Task<Result<string>> ResetPasswordAsync(string username, string code, string newPassword);

    }
}