using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_authentication__api.src.Application.DTOs
{
    public class SignUpRequestDTO
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string ExternalReferenceId { get; set; }
    }
    public class TwoFactorRequestDTO
    {
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class TwoFactorCodeDTO
    {
        public string Username { get; set; }
        public string Code { get; set; }
    }

    public class PasswordRecoveryRequestDTO
    {
        public string Username { get; set; }
    }

    public class PasswordResetDTO
    {
        public string Username { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
    }
    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}