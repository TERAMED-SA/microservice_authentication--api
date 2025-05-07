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
        /// Cria um novo Patient (apenas para Admins ou Managers).
        /// </summary>
        [HttpPost("register/patient")]
        [Authorize(Policy = PolicyNames.OnlyManager)]
        public async Task<IActionResult> RegisterPatient([FromBody] SignUpRequestDTO dto)
        {
            var result = await _authenticationService.RegisterPatientAsync(dto);
            return StatusCode(result.Code, result);
        }

        /// <summary>
        /// Cria um novo Professional (apenas para Admins ou Managers).
        /// </summary>
        [HttpPost("register/professional")]
        [Authorize(Policy = PolicyNames.OnlyManager)]
        public async Task<IActionResult> RegisterProfessional([FromBody] SignUpRequestDTO dto)
        {
            var result = await _authenticationService.RegisterProfessionalAsync(dto);
            return StatusCode(result.Code, result);
        }
    }
}