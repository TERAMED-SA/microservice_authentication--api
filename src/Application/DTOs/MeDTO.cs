using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_authentication__api.src.Application.DTOs
{
    public class MeDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ExternalReferenceId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IList<string> Roles { get; set; } = [];
    }
}