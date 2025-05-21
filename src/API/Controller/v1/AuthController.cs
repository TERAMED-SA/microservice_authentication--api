using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using microservice_authentication__api.src.Application.DTOs;
using microservice_authentication__api.src.Application.Services;
using microservice_authentication__api.src.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace microservice_authentication__api.src.API.Controller.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v1/[controller]")]
    public class AuthController(IAuthenticationService authenticationService) : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;

        /// <summary>
        /// Autentica um usuário e retorna o token JWT.
        /// </summary>
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestDTO request)
        {
            var result = await _authenticationService.SignInAsync(request);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Retorna os dados do usuário autenticado.
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
            var result = await _authenticationService.Me(token);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Cria um novo usuário Admin (apenas para Admins autenticados).
        /// </summary>
        [HttpPost("register/admin")]
        [Authorize(Policy = PolicyNames.OnlyAdmin)]
        public async Task<IActionResult> RegisterAdmin([FromBody] SignUpRequestDTO dto)
        {
            var result = await _authenticationService.RegisterAdminAsync(dto);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Cria um novo Manager (apenas para Admins).
        /// </summary>
        [HttpPost("register/manager")]
        [Authorize(Policy = PolicyNames.OnlyAdmin)]
        public async Task<IActionResult> RegisterManager([FromBody] SignUpRequestDTO dto)
        {
            var result = await _authenticationService.RegisterManagerAsync(dto);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Cria um novo Patient.
        /// </summary>
        [HttpPost("register/patient")]
        public async Task<IActionResult> RegisterPatient([FromBody] SignUpRequestDTO dto)
        {
            var result = await _authenticationService.RegisterPatientAsync(dto);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Cria um novo Professional
        /// </summary>
        [HttpPost("register/professional")]
        public async Task<IActionResult> RegisterProfessional([FromBody] SignUpRequestDTO dto)
        {
            var result = await _authenticationService.RegisterProfessionalAsync(dto);
            return StatusCode(result.Code, result);
        }
        /// <summary>
        /// Activar o 2f
        /// </summary
        [HttpPost("enable-2fa")]
        public async Task<IActionResult> EnableTwoFactor([FromBody] TwoFactorRequestDTO dto)
        {
            var result = await _authenticationService.EnableTwoFactorAsync(dto.Username, dto.PhoneNumber);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Confirmar o 2f
        /// </summary
        [HttpPost("confirm-2fa")]
        public async Task<IActionResult> ConfirmTwoFactor([FromBody] TwoFactorCodeDTO dto)
        {
            var result = await _authenticationService.ConfirmTwoFactorPhoneAsync(dto.Username, dto.Code);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Confirmar o codigo do 2f
        /// </summary
        [HttpPost("verify-2fa")]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] TwoFactorCodeDTO dto)
        {
            var result = await _authenticationService.VerifyTwoFactorCodeAsync(dto.Username, dto.Code);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Solicita a recuperação de senha (envia código por SMS ou email)
        /// </summary>
        [HttpPost("requestResetPassword")]
        public async Task<IActionResult> RequestRecovery([FromBody] PasswordRecoveryRequestDTO dto)
        {
            var result = await _authenticationService.RequestPasswordRecoveryAsync(dto.Username);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Reseta a senha com base no código enviado
        /// </summary>
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDTO dto)
        {
            var result = await _authenticationService.ResetPasswordAsync(dto.Username, dto.Code, dto.NewPassword);
            return StatusCode(result.Code, result);
        }
        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            var username = User.Identity!.Name;
            var result = await _authenticationService.ChangePasswordAsync(username!, dto.CurrentPassword, dto.NewPassword);
            return StatusCode(result.Code, result);
        }
    }
}