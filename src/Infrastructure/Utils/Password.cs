using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace microservice_authentication__api.src.Infrastructure.Utils
{
    public static class Password
    {
        public static string GenerateSecurePassword(int length = 12)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*()_-+=<>?";

            var random = new Random();

            var password = new StringBuilder();
            password.Append(upper[random.Next(upper.Length)]);
            password.Append(lower[random.Next(lower.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(special[random.Next(special.Length)]);

            string allChars = upper + lower + digits + special;
            for (int i = password.Length; i < length; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Embaralha para não deixar os primeiros caracteres previsíveis
            return new string([.. password.ToString().ToCharArray().OrderBy(_ => random.Next())]);
        }
    }
}