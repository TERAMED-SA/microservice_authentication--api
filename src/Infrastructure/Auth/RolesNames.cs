using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_authentication__api.src.Infrastructure.Auth
{
    public class RolesNames
    {
        public static string Admin => "Admin";
        public static string Manager => "Manager";
        public static string Professional => "Professional";
        public static string Patient => "Patient";
        public static string Scheduling => "Scheduling";
        public static string Appointment => "Appointment";
    }
}