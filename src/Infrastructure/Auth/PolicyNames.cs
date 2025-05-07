using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_authentication__api.src.Infrastructure.Auth
{
    public class PolicyNames
    {
        public const string OnlyAdmin = "AdminOnly";
        public const string OnlyManager = "ManagerOnly";
        public const string OnlyProfessional = "ProfessionalOnly";
        public const string OnlyPatient = "PatientOnly";
    }
}